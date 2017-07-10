#!/bin/bash

###################################################################################
# This script is run for use on a Samsung UPC STB.
#
# This script is used with application running from flash
#
# CHANGE HISTORY
#
# SV 04/01/11   update for Samsun GW
###################################################################################

echo "** FACTORY RESET **"

ERASE_FLASH=1

while [ $# -gt 0 ]
do
    PARAM=$1

    case "$PARAM" in
        NO_FLASH)
            ERASE_FLASH=0
        ;;
    esac

    shift
done

delete_partitions()
{
    cat<<EOM | fdisk /dev/sda

d
1
d
2
d
w
EOM

}

create_partitions()
{
    cat<<EOM | fdisk /dev/sda

n
p
1

+512M
n
p
2

+4096M
n
p
3


t
3
1
w
EOM

}

format_partitions()
{
    echo "Format sda1..."
    mkfs.ext3 /dev/sda1
    tune2fs -c0 /dev/sda1

    echo "Format sda2..."
    mkfs.ext3 /dev/sda2
    tune2fs -c0 /dev/sda2

    echo "Format sda3..."
    /NDS/drivers/xtvfs_format /dev/sda3 p
}

if [ ! -f "/NDS/drivers/xtvfs_format" ]; then
  echo "/NDS/drivers/xtvfs_format not found, quit script"
  exit -1
fi

echo "**   Killing processes ... "
ps | grep "MW_Process" | sed -n -e "1p" | sed "s/ *//" | cut -f1 -d" " | xargs kill -9 ;
ps | grep "APP_Process" | sed -n -e "1p" | sed "s/ *//" | cut -f1 -d" " | xargs kill -9 ;
ps | grep "SCD_Process" | sed -n -e "1p" | sed "s/ *//" | cut -f1 -d" " | xargs kill -9 ;
ps | grep "CA_Process" | sed -n -e "1p" | sed "s/ *//" | cut -f1 -d" " | xargs kill -9 ;
ps | grep "PWM_Process" | sed -n -e "1p" | sed "s/ *//" | cut -f1 -d" " | xargs kill -9 ;
ps | grep "STM_Process" | sed -n -e "1p" | sed "s/ *//" | cut -f1 -d" " | xargs kill -9 ;
#ps | grep "XTVFS" | sed -n -e "1p" | sed "s/ *//" | cut -f1 -d" " | xargs kill -9 ;
sleep 5

umount /mnt/nds/dev_2/part_0
umount /mnt/nds/dev_3/part_0

#if [[ "$ERASE_FLASH" = "1" ]]
#then
#    echo "**   Erasing Flash ... " 
#    /sbin/stl.format /dev/bml1/1
#    /sbin/mkfs.ext3 /dev/stl1/1
#    tune2fs -c0 /dev/stl1/1
#fi

if [[ "$ERASE_FLASH" = "1" ]]
then
	flash_node="/dev/flash_symlnk0"
	
    # temporary workaround for Samsung
    if [ ! -f ${flash_node} ] ; then
        ln -s /dev/stl1/1 ${flash_node}
    fi	

    echo "**   Erasing Flash ... "
    /sbin/mkfs.ext3 ${flash_node}
    tune2fs -c0 ${flash_node}
fi

echo "**   Unmounting partitions"
umount /mnt/nds/dev_4/part_0
umount /mnt/nds/dev_4/part_1
umount /mnt/nds/dev_4/part_2

echo "**   Deleting partitions"
#delete_partitions
#create_partitions
#format_partitions
/NDS/bin/disk_partition

echo "** DONE **"


# End of file.

