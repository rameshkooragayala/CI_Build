#!/bin/bash

###################################################################################
# This script is run for use on a 7335 STB.
#
# This script assumes that the $HOME/output/${NDS_PLATFORM} directory on the
# host is mounted as $host_root on the 7335 via NFS.
#
# CHANGE HISTORY
#
# AH 27/11/07 Updated initial CMS script to allow conditional control of certain
#             startup operations so that the unified script can be used by more
#             people.
# AH 07/12/07 Add parameter to allow NVRAM to not be cleaned
# AH 07/12/07 Make partitions created only if partition is to be formatted
# AH 07/12/07 Set XTVFS SDA2 partition to 100M (until we can use rest of disk)
# AH 07/12/07 Remove copy of libgcc_s.so as this should never be required (legacy cygwin)
# MM 16/01/08 Add workaround CQ171068 to prevent kernel crash
# MM 16/01/08 Update to call monitor_progress script which will kill middleware processes
#             when test results are present.  This is a temporary solution until a better
#             home for the functionality can be found.
# MM 02/05/08 Add ulimit -c unlimited into run.sh
# DS 11/05/10 Add NODELETE, NOCLEAN_DB, and NOCLEAN_REC
# BT 11/08/10 Adapt for SH shell, change NVRAM dd command to manage 127 blocks of 512 bytes (MHXNVRAMFS manages 64Kbytes now), remove Flash Data access.
# BT 12/08/10 Add 3rd HDD partition for UPC Samsung board
# BT 25/08/10 Modify dd command to manage a 2048 bytes long structure (BSL<=>MW)
###################################################################################

echo "******************************"

build=$1; shift;
host_root=$1; shift;

FORMAT="false"
NOCLEAN="false"
NODELETE="false"
NOCLEAN_DB="false"
NOCLEAN_REC="false"
NOCLEAN_NVRAM="false"
FORMAT_FLASH="false"
NORUN="false"
WITHOUT_INSTALL_BUILD="0"
TEST_TIMEOUT=0
DELETE_PARTITIONS="false"
DELETE_PARTITION_2="false"
WITH_CDI_TRACKER=0
NDS_MW_GID=10000
BSLCOMM_MOCK=0
WITH_IPV6=0

grep "ip=dhcp" /proc/cmdline
if [ "$?" -eq "0" ]
then
  nfs_rootfs=1
else
  nfs_rootfs=0
fi

while [ $# -gt 0 ]
do

    CURRENT_PARAMETER=$1

    case "$CURRENT_PARAMETER" in
        FORMAT)
            echo "*** FORMAT SELECTED        ***"
            FORMAT="true"
            ;;
        NODELETE)
            echo "*** NODELETE SELECTED       ***"
            NODELETE="true"
            ;;
        NOCLEAN)
            echo "*** NOCLEAN SELECTED       ***"
            NOCLEAN="true"
            ;;
        NOCLEAN_DB)
            echo "*** NOCLEAN DB SELECTED  ***"
            NOCLEAN_DB="true"
            ;;
        NOCLEAN_REC)
            echo "*** NOCLEAN REC SELECTED  ***"
            NOCLEAN_REC="true"
            ;;
        NOCLEAN_NVRAM)
            echo "*** NOCLEAN NVRAM SELECTED  ***"
            NOCLEAN_NVRAM="true"
            ;;
        NOCLEAN_NVRAM0)
            echo "*** NOCLEAN NVRAM0 SELECTED  ***"
            NOCLEAN_NVRAM="true"
            ;;			
        NORUN)
            echo "*** NORUN SELECTED         ***"
            NORUN="true"
            ;;
        DELETE_PARTITIONS)
            echo "*** DELETE_PARTITIONS SELECTED        ***"
            DELETE_PARTITIONS="true"
            ;;
        DELETE_PARTITION_2)
            echo "*** DELETE_PARTITION_2 SELECTED        ***"
            DELETE_PARTITION_2="true"
            ;;           
        TEST_TIMEOUT=*)
            echo "*** TEST TIMEOUT SET NB THIS IS FOR CI ONLY !!! ***"
            TEST_TIMEOUT=$1
            ;;
	WITHOUT_INSTALL_BUILD)
	    echo "*** WITHOUT INSTALL BUILD SELECTED ***"
	    WITHOUT_INSTALL_BUILD=1
            ;;
	WITH_CDI_TRACKER)
	    echo "*** WITH CDI TRACKER ***"
	    WITH_CDI_TRACKER=1
            ;;
        FORMAT_FLASH)
            echo "*** Formatting flash ***"
            FORMAT_FLASH="true"
            ;;
	BSLCOMM_MOCK)
            echo "*** WITH BSLCOMM_MOCK ***"
            BSLCOMM_MOCK=1
            ;;
        FORMAT_RF4CE)
            echo "*** Formatting RF4CE data ***"
            FORMAT_RF4CE="true"
            ;;
        WITH_IPV6)
            echo "*** With IPv6 ***"
            WITH_IPV6=1
            ;;
    esac

    shift

done

echo "******************************"

disks="sda sdb sdc sdd"
flash_node="/dev/flash_symlnk0"

# this will find the first non usb disk present
find_sata_disk() {
  disk=""
  disk_present=0
  for d in $disks
  do
    readlink /sys/block/$d |grep -v usb &>/dev/null
    if [ $? -eq 0 ] ; then
        disk=$d
        disk_present=1
        echo "found sata disk on $d"
        break
    fi
  done
}

change_root_permissions()
{
    echo "switching permissions of $1 root"
    mkdir -p /mnt/tmp_mount
    mount $1 /mnt/tmp_mount
    chown root.${NDS_MW_GID} /mnt/tmp_mount
    chmod 770 /mnt/tmp_mount
    umount /mnt/tmp_mount
    rm -rf /mnt/tmp_mount
}

# Create all of the directories we're going to need (all in RAMFS) - so we can
# see where they're all coming from.
create_directories()
{

    echo "create_directories"

    mkdir -p /fifo               # this is required to allow IPC to work
    mkdir -p /root
}

enable_ipv6()
{
    if [ ${WITH_IPV6} -eq 1 ]
    then
       # allow original RA
       echo 1 > /proc/sys/net/ipv6/conf/all/disable_ipv6
       echo 1 > /proc/ipv6_original_ra
       echo 0 > /proc/sys/net/ipv6/conf/all/disable_ipv6
    fi
}

# Load the kernel modules, NVRAM superblock and the libgcc library.
load_kernel_modules()
{
    echo "load_kernel_modules"

    echo "##################### LOAD DRIVERS ###########################"
     
    if [[ "$NOCLEAN" = "false" && "$NOCLEAN_NVRAM" = "false" ]]
    then
        echo "erasing nvram0"
        dd of=/dev/nds/nvram0 if=/dev/zero count=60 bs=512 seek=4
    fi

    insmod $host_root/drivers/xtvfs.ko
    insmod $host_root/drivers/mhxnvramfs.ko
    
    xtvfs_pid=$(pidof XTVFS)
    if [ -z "$xtvfs_pid" ] ; then
        echo "Couldn't find XTVFS thread !!"
    else 
        echo "Bumping XTVFS priority"
        chrt -p -r 91 $xtvfs_pid
    fi
}

#copy partition format and checking utility tools in storage_helper's tool search path.
{
    echo "copy_xtvfs_format_utility"
    rm -f /bin/xtvfs_format
    ln -sf $host_root/drivers/xtvfs_format /bin/xtvfs_format
}  

delete_unused_partitions()
{
    if [ "$disk_present" -eq "1" ]
	then
    echo "delete_unused_partitions"

    cat<<EOM | fdisk /dev/${disk}

d
4
w
EOM
	fi

}

format_partition_FLASH()
{
    /sbin/mkfs.ext3 ${flash_node}
    tune2fs -c0 ${flash_node}
    change_root_permissions "${flash_node}"
}

delete_partition_1()
{
    if [ "$disk_present" -eq "1" ]
	then
    echo "Deleting partition 1"

    cat<<EOM | fdisk /dev/${disk}

d
1
w
EOM
    fi
}

delete_partition_2()
{
    if [ "$disk_present" -eq "1" ]
	then
    echo "Deleting partition 2"

    cat<<EOM | fdisk /dev/${disk}

d
2
w
EOM
    fi
}

delete_partition_3()
{
    if [ "$disk_present" -eq "1" ]
	then
    echo "Deleting partition 3"

    cat<<EOM | fdisk /dev/${disk}

d
3
w
EOM
    fi
}

#sda1 cannot be recreated independently of sda2.
# Therefore always call BOTH delete_partition_1 and delete_partition_2 before calling this function.
create_partition_sda1()
{
    if [ "$disk_present" -eq "1" ]
	then
    echo "create_partition_sda1"

    cat<<EOM | fdisk /dev/${disk}

n
p
1

+512M
w
EOM
    fi
}

# Always call delete_partition_2 before calling this function.
create_partition_sda2()
{
    if [ "$disk_present" -eq "1" ]
	then
    echo "create_partition_sda2"

    cat<<EOM | fdisk /dev/${disk}

n
p
2

+4096M
w
EOM
    fi
}

create_partition_sda3()
{
    if [ "$disk_present" -eq "1" ]
	then
    echo "create_partition_sda3"

    cat<<EOM | fdisk /dev/${disk}

n
p
3


t
3
1
w
EOM
    fi
}

print_partition_table()
{
    if [ "$disk_present" -eq "1" ]
	then
    echo "Current state of partition table is printed below:"

    cat<<EOM | fdisk /dev/${disk}

p
q
EOM
    fi
}

# Format the EXT3 partition
format_sda1()
{
    if [ "$disk_present" -eq "1" ]
	then
        echo "format_sda1"

        mkfs.ext3 /dev/${disk}1
        tune2fs -c0 /dev/${disk}1
        change_root_permissions "/dev/${disk}1"
    fi
}

# Format the EXT3 partition
format_sda2()
{
    if [ "$disk_present" -eq "1" ]
	then
        echo "format_sda2"

        mkfs.ext3 /dev/${disk}2
        tune2fs -c0 /dev/${disk}2
        change_root_permissions "/dev/${disk}2"
    fi
}


# 	 the EXT3 filesystem, and clean db only
# filesystem is unmounted as PDM will mount when middleware is started
mount_and_clean_db()
{
    if [ "$disk_present" -eq "1" ]
	then
    echo "mount_and_clean_db"
# we are dropping sda1, it is a partition to store Client images.
# Maybe we shall erase it as well in the future; but for the moment there is no defined mount point.
    e2fsck -p /dev/${disk}2
    mkdir /mnt/app_data       # temporary mount point for DATA file sysem
    mount /dev/${disk}2 /mnt/app_data
    rm -rf /mnt/app_data/*
    umount /mnt/app_data
    fi
}

# Format the XTVFS partition
format_sda3()
{
    if [ "$disk_present" -eq "1" ]
	then
    echo "format_sda3"

    $host_root/drivers/xtvfs_format /dev/${disk}3 p
    fi
}

# Mount the XTVFS filesystem, and clean recordings only manually.
# filesystem is unmounted as PDM will mount when middleware is started
mount_and_clean_rec()
{
    if [ "$disk_present" -eq "1" ]
	then
    echo "mount_and_clean_rec"

    mkdir /mnt/app_recording  # temporary mount point for RECORDING file sysem
    mount -t xtvfs /dev/${disk}3 /mnt/app_recording
    rm -rf /mnt/app_recording/*
    umount /mnt/app_recording
    fi
}

# this step is necessary to make sure that the partition table is backed up to prevent startup issues
backup_partition_table()
{
    if [ "$disk_present" -eq "1" ]
	then
# Not sure this can be used, sda1 is reserved for BSL...
    dd if=/dev/zero of=/dev/${disk} bs=512 count=1 seek=6
    fi
}

# Copy the NDS directory from the host, via NFS, into memory.
install_build()
{

    echo "install_build"

    rm -rf /NDS/*
    #use cp -a here to perform recursive copy, preserve symlinks and timestamps
    cp -a $host_root/$build/fs/NDS/* /NDS
    rm -rf /NDS/results

}

dhcp_only_dns()
{
    echo "dhcp_only_dns"
    cat <<EOF >/tmp/udhcpc.dns_only
#!/bin/sh
RESOLV_CONF="/etc/resolv.conf"

case "\$1" in
        renew|bound)
                echo -n >> \$RESOLV_CONF
                [ -n "\$domain" ] && echo search \$domain >> \$RESOLV_CONF
                for i in \$dns ; do
                        echo adding dns \$i
                        echo nameserver \$i >> \$RESOLV_CONF
                done
                ;;
esac

exit 0
EOF

    chmod 755 /tmp/udhcpc.dns_only

    udhcpc -b -q -s /tmp/udhcpc.dns_only

    chown root.$NDS_MW_GID /etc/resolv.conf
    chown root.$NDS_MW_GID /tmp/resolv.conf
    chmod 770 /tmp/resolv.conf
    chmod 770 /etc/resolv.conf
}

set_permissions()
{
    echo "Setting permissions on devices"

    devices="
/dev/sec
/dev/scard0
/dev/devmem
/dev/sda*
/dev/sdb*
/dev/stl*/*
/dev/bml*/*
/dev/null
/dev/dri/card0
/dev/mmc*
"

    for d in $devices ;
    do
      chmod 660 $d
      chown root.${NDS_MW_GID} $d
    done

    chmod -R ug+rw /vfs
    chown -R root.${NDS_MW_GID} /vfs

    chmod 777 /etc
    # for DMS/CI logging
    chmod 777 /dev/console

    chown root.$NDS_MW_GID /etc/resolv.conf
    chown root.$NDS_MW_GID /tmp/resolv.conf
    chmod 770 /tmp/resolv.conf
    chmod 770 /etc/resolv.conf

    # workaround for DAL creating /dev/nds/fifo from CA_Process
    chmod 775 /dev/nds

    mount -t tmpfs nodev /mnt
}

# call any pre-run scripts and export symbol that will run
prepare_to_run()
{

    echo "prepare_to_run"

    # Change driver and middlware thread priorities.
    # Must run from directory containing the script (unfortunately).

    rm -fr /fifo/*

    cd $host_root/drivers

    if [ "${nfs_rootfs}" == "1" ];
    then
        dhcp_only_dns
    fi

    # Finally, start the SCD process.

    cd /root # somewhere writeable in case we generate a core file
    echo "ulimit -c unlimited" > run.sh

    PRELOAD=/lib/libdl.so:libnds_eglext.so:${host_root}/drivers/libprio_range.so:${host_root}/drivers/libResolveRefresh.so
    if [[ "$BSLCOMM_MOCK" = "1" ]]
    then
    	PRELOAD="${host_root}/drivers/bslcomm.so:$PRELOAD"
    fi
    LIB_PATH=/NDS/drivers:/lib/modules:/NDS/lib
	

    if [ "$WITH_CDI_TRACKER" -eq "1"  ]
	then
	PRELOAD="$host_root/drivers/cdi_tracker.so:$PRELOAD"
    fi

    if [ -f ${host_root}/fs/NDS/bin/mw_starter_process ] ; then
        start_process=mw_starter_process
    else
        start_process=SCD_Process
    fi

    echo "export FAKE_BSLCOMM_PATH=${host_root}/bslcomm.bin; export LD_PRELOAD=$PRELOAD ; export LD_LIBRARY_PATH=${LIB_PATH} ; /NDS/bin/${start_process}" >> run.sh
	
    chmod 777 run.sh
    cat run.sh

    echo "starting firewall"
    ${host_root}/drivers/firewall.sh
    ${host_root}/drivers/firewall-ipv6.sh

    cat /dev/nds/versioninformation |grep FOS_SW_VER|awk -F ":" '{print "FOS Baseline: "$2}'
}

copy_config_files()
{
	echo "Copy config files"
	cp $host_root/drivers/fusion_os_cdi_labels.cfg /NDS/config &>/dev/null
	cp $host_root/drivers/fusion_os_storage_labels.cfg /NDS/config &>/dev/null
}


fix_flash_device_node()
{
    echo "fixing flash device node"
    # temporary workaround for Samsung

    if [ ! -f ${flash_node} ] ; then
        ln -s /dev/stl1/1 ${flash_node}
    fi
}

# Allow core files to be created.
echo "ulimit -c unlimited" >> /etc/profile
ulimit -c unlimited

# start script logic
find_sata_disk
create_directories       # create directories required to support middleware
copy_config_files        # Copy the config files from $host_root/drivers/fusion_os*.cfg to /NDS/config/
load_kernel_modules      # loads kernel modules, NVRAM SB and libgcc library

# for all cefdk
fix_flash_device_node

set_permissions
enable_ipv6

echo "\n127.0.0.1 localhost.localdomain localhost" >> /etc/hosts

echo "disable udev"
killall udevd

# make RR take over the world
echo "-1" >/proc/sys/kernel/sched_rt_runtime_us 

echo "Cpuinfo content"
cat /proc/cpuinfo

echo "Initial state of partition table:"
print_partition_table


# Logic to format partitions when requested
# supports total format, or independent formatting of either partition
# Note that if formatting sda1 alone, the partition will not be re-made.
# It has to stay the same size as it currently is on the disk because sda1 and sda2 are contiguous.
# We can't make sda1 bigger than the space between the start of the disk and the start of sda2.
# sda2 can be remade independently of sda1 because it is the last partition on the disk.
if [ "$disk_present" -eq "1" ]
then
  if [[ "$FORMAT" = "true" ]]
	then
          format_partition_FLASH

          # make sure config file is there
          [ "$WITHOUT_INSTALL_BUILD" -eq 0 ] && mount --bind $host_root/fs/NDS /NDS
          /NDS/bin/disk_partition
          [ "$WITHOUT_INSTALL_BUILD" -eq 0 ] && umount /NDS

	  echo "State of partition table after formatting the disk:"
	  backup_partition_table
	  print_partition_table
  fi

  if [[ "$NOCLEAN" = "false" || "$NOCLEAN_DB" = "false" || "$NOCLEAN_REC" = "false" ]]
  then
    if [[ "$NOCLEAN" = "false" && "$NOCLEAN_DB" = "false" ]]
    then
        if [ "$FORMAT" = "false" ]
        then
            mount_and_clean_db            # manually clean the db
        fi
    fi

    if [[ "$NOCLEAN" = "false" && "$NOCLEAN_REC" = "false" ]]
    then
        if [ "$FORMAT" = "false" ]
        then
            mount_and_clean_rec            # manually clean the recordings
        fi
    fi
  fi

  if [[ "$NOCLEAN" = "true" || "$NOCLEAN_DB" = "true" ]]
  then
     ls -l /dev/${disk}* &>/dev/null
     e2fsck -p /dev/${disk}2
     e2fsck -p ${flash_node}
  fi
else
  if [[ "$FORMAT" = "true" ]]
	then
          format_partition_FLASH
  fi
fi

if [[ "$FORMAT_FLASH" = "true" ]]
then
        format_partition_FLASH
fi

if [[ "${FORMAT_RF4CE}" = true ]]
then
    echo "Erasing rf4ce partition table"
    ${host_root}/drivers/rf4ce_diag -r 
fi

if [[ "$DELETE_PARTITIONS" = "true" ]]
then
	format_partition_FLASH
        dd if=/dev/zero of=/dev/sda bs=512 count=1
fi

if [[ "$DELETE_PARTITION_2" = "true" ]]
then
  delete_partition_3
fi
    
if [ "$WITHOUT_INSTALL_BUILD" -eq 0 ]
then
    install_build          # copy the /NDS filesystem to the ram filesystem
fi

# this create core.pid files
echo 1 > /proc/sys/kernel/core_uses_pid

prepare_to_run

if [ "$NORUN" = "false" ];
then
    echo "Starting test . . ."

    # if timeout argument is used, set the test to timeout (used to timeout tests in CI)
    if [[ $TEST_TIMEOUT != 0 ]]
    then
        TEST_TIMEOUT=`echo $TEST_TIMEOUT | sed 's/TEST_TIMEOUT=//g'`
        echo "TEST_TIMEOUT is: ${TEST_TIMEOUT}"
    fi

    # call monitor script which returns to shell when test is complete
    if [[ -f ../host/monitor_progress.sh ]];
    then
        echo "Calling monitor_progress.sh"
        ../host/monitor_progress.sh $TEST_TIMEOUT &
    fi

    # run the test
    /root/run.sh
fi

# End of file
