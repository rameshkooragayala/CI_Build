#!/bin/sh
echo "-------------------------------------"
echo "UPC Horizon Integration Launch script"
echo "                        Debug version"
echo "start_dbg.sh ------------------------"
echo "-------------------------------------"
if [ -f /NDS.tar.bz2 ]; then
    echo "remove NDS.tar.bz2"
    rm /NDS.tar.bz2
fi

if [ -f /NDS/boot_info.sh ]; then
  chmod 777 /NDS/boot_info.sh
  /NDS/boot_info.sh
fi

# delay launch and set time through rdate
#current_dir=$(pwd)
#cd /NDS/
#./set_rdate.sh
#cd $current_dir
#Change in start.sh to transmit 'FORMAT'
###### Moves to flash if needed ########
if [ -f /NDS/nds_folders_to_flash_partition.sh ]; then
  chmod 777 /NDS/nds_folders_to_flash_partition.sh
  /NDS/nds_folders_to_flash_partition.sh $1
fi


echo -e "\noneconfig information:"
oneconfig.exe all
echo -e "\nMemory information:"
cat /proc/meminfo
echo ""

############################
##### Script functions #####
############################

#-----------------------------------------------------------------------------------------------------
# USB Launcher
# must be called after check_usb_config_fct() to ensure that $sd_usb} is properly set
#-----------------------------------------------------------------------------------------------------
usb_launcher_fct() {
echo "----------------- USB Launcher -----------------------"
if [ -b /dev/${sd_usb}1 ]
then
    echo "Try to mount the USB key on /mnt/usb..."
    if [ ! -d /mnt/usb ]
    then
        mkdir /mnt/usb
    fi
    mount -o sync /dev/${sd_usb}1 /mnt/usb
fi
}

#-----------------------------------------------------------------------------------------------------
#XBMC Management
#-----------------------------------------------------------------------------------------------------
xbmc_fct() {
echo "----------------- XBMC Management -----------------------"
if [ "$IS_XBMC_CMDLINE_DETECTED" == "YES" -a  "$ISDOCSISBUILD" == "NO" ]
then
   echo "Change working directory to xbmc."
   cd /NDS/xbmc/
   ./xbmc_helper_tool.bin
elif [ $XBMC == "ON"  -a  "$ISDOCSISBUILD" == "YES" ]
then
   echo "Change working directory to xbmc."
   cd /NDS/xbmc/
   ./xbmc_helper_tool.bin
else
     echo "Xbmc disabled"
fi
}

#-----------------------------------------------------------------------------------------------------
#NASC Management
#-----------------------------------------------------------------------------------------------------
nasc_fct() {
# WP1188 requirement
echo "------- [WP1188]Patch for FOS CQ 1526777 -------"

chmod 775 /
chmod 777 -R /dev
chmod 777 -R /lib
chmod 777 -R /usr
chmod 777 -R /sbin
chmod a+rw /var/run/fifo/ngi_event/*

# Change permissions of /NDS in FLASH mode only (in order to access this path in NFS mode)
# Do we really need to do it ==> not clearly mentionned in WP1188
if [ "$EXECFROM" == "FLASH" ]
then
   echo "time chmod -R 550 /NDS"
   time chmod -R 550 /NDS
fi
# NDS_MW comes from /etc/group
echo "time chown -R root.NDS_MW /NDS"
time chown -R root.NDS_MW /NDS
}

#-----------------------------------------------------------------------------------------------------
#check config in a line (for check_usb_config_fct())
#Usage: check_config_line line_num line_string
#-----------------------------------------------------------------------------------------------------
check_config_line() {
    lnm=$1
    line=$2
    #echo "line $lnm is $line"
    if [ "$line" != "" ]; then
      line=`echo $line | awk '{print $1}' | awk -F "\#" '{print $1}'`
      #echo "line after awk is [$line]"
      if [ "$line" != "" ];then
        line=`echo $line | grep '^[0-9a-zA-Z_]\{1,\}=*'`
      fi
    fi
    if [ "$line" != "" ];then
      next_variable=`echo $line | awk -F "=" '{print $1}'`
      next_value=`echo $line | awk -F "=" '{print $2}'`
      if [ "$next_variable" != "" ] && [ "$next_value" != "" ]; then
        echo "$next_variable=$next_value"
        eval $next_variable=$next_value
      else
        echo "not defined $next_variable=$next_value"
      fi
    #else
    #  echo "line $lnm is invalid"
    fi
}

#-----------------------------------------------------------------------------------------------------
# Manage log files including diag logs, diag binary logs, kernel logs.
# All log files within one running are stored in a directory named /mnt/usb/log/run_$(run_count), here $run_count is the number of running
# Diag log:           /mnt/usb/log/run_${run_count}/diag_${run_count}.log
# Diag binary log:    /mnt/usb/log/run_${run_count}/*.CUR, /mnt/usb/run_${run_count}/*.LOG
# Kernel log:         /mnt/usb/log/run_${run_count}/kernel_${run_count}.log
#-----------------------------------------------------------------------------------------------------
manage_usb_logs() {

if [ "$USB_PRESENCE" != "YES" ]; then
  return
fi

#manage diag log
if [ $USB_AVAILABLE_SPACE -lt 100000 ]; then
  NEEDMOUNTUSB="NO"
  echo "USB disk available space less than 100MB"
  echo "disable log in USB"
  USEUSBLOG="NO"
  return
fi

#find run count
run_count=0
  run_numbers=`ls /mnt/usb/log | grep run_ | sed -e 's/run_//g'`
  for num in $run_numbers
  do
    if [ $num -ge $run_count ]; then
      run_count=$((num+1))
    fi
    gzip /mnt/usb/log/run_${num}/*.log &> /dev/null || continue
  done
echo "check run count is $run_count"
mkdir -p /mnt/usb/log/run_${run_count}
USBLOGROOT="/mnt/usb/log/run_"${run_count}
echo "usb log in $USBLOGROOT"

#mange diag log on USB
if [ "$USBLOG" == "YES" ]; then
  echo "USBLOG is set as YES.."
  LOG_FILE="$USBLOGROOT/diag_${run_count}.log"
  NEEDMOUNTUSB="YES"
  USEUSBLOG="YES"
fi

#manage .CUR files in USB, make sure not to lose the log files in long run tests
if [ "$USBLOG" == "YES" ] && [ -x /mnt/usb/NDS/usb_script.sh ];then
  dos2unix /mnt/usb/NDS/usb_script.sh
  echo "usb_script is present to execute in background"
  NEEDMOUNTUSB="YES"
  EXEC_USB_SCRIPT="/mnt/usb/NDS/usb_script.sh"
fi

# manage diag binary log file on USB
if [ "$DIAGBINUSBLOG" == "YES" ]; then
 echo "DIAGBINUSBLOG is set as YES.."
 sed -i 's/DIR_LOG_PATH=.*/DIR_LOG_PATH="\/mnt\/usb\/log\/run_'${run_count}'\/"/g' /NDS/config/diag_binary.cfg
 NEEDMOUNTUSB="YES"
fi # if [ "$DIAGBINUSBLOG" == "YES" ]; then


# manage kernel log on USB
if [ "$USB_PRESENCE" == "YES" ] && [ "$KERNELUSBLOG" == "YES" ]; then
  echo "KERNELUSBLOG is set as YES.."
  KERNEL_LOG_FILE="$USBLOGROOT/kernel_${run_count}.log"
  NEEDMOUNTUSB="YES"
fi

# check gdl usb log debug
if [ "$GDLUSBLOG" == "YES" ]; then
  NEEDMOUNTUSB="YES"
  GDL_LOG_FILE="$USBLOGROOT/gdl_${run_count}.log"
fi

#enable CDI logs........ @ /mnt/usb/run_($run_count)/cdi_logs_${run_count}....
if [ "$USB_PRESENCE" == "YES" ] && [ "$CDIUSBLOG" == "YES" ]; then
  echo "CDIUSBLOG is set as YES..."
  CDI_LOG_DIR="$USBLOGROOT/cdi_logs_${run_count}"
  NEEDMOUNTUSB="YES"
fi
}


#-----------------------------------------------------------------------------------------------------
# Check config files in USB.
# WARNING: this function must be called before binary_cfg_file_fct. Because it will check and
#          replace the /NDS/config/* with the one exists in /mnt/usb/NDS/config/*.
#-----------------------------------------------------------------------------------------------------
check_usb_config_fct() {
  USBLOG="NO" #config in app_config_cfg
  USEUSBLOG="NO"  #control variable

  sd_list="sda sdb"
  sd_usb=sdb

  #USB Key Device Detection
  for d in $sd_list
  do
    readlink /sys/block/$d | grep usb &>/dev/null
    if [ $? -eq 0 ] ; then
        sd_usb=$d
        break
    fi
  done

  echo "---------- Debug : check config files from usb key ------------"
  usb_mount=`mount | grep /dev/${sd_usb}1 | grep /mnt/usb &> /dev/null`
  if [ $? == 0 ] ;then
    USB_PRESENCE="YES"
  else
    USB_PRESENCE="NO"
  fi

  if [ "$USB_PRESENCE" = "NO" ];then
    result=`mkdir /mnt/usb; mount -o sync /dev/${sd_usb}1 /mnt/usb 2>/dev/null`
    if [ $? != 0 ];then
      echo "No USB key detected"
    else
      USB_PRESENCE="YES"
    fi
  fi

  BUILD_OPT=$(cat /NDS/options_start)

  if [ "$USB_PRESENCE" = "YES" ]; then

    #check USB config
    if [ -f /mnt/usb/NDS/app_start.cfg ]; then
      echo "load configs in /mnt/usb/NDS/app_start.cfg"
      lnm=0
      while read line
      do
        lnm=$((lnm+1))
        check_config_line $lnm $line
      done < "/mnt/usb/NDS/app_start.cfg"
      #handle the last line not read by 'read'
      #echo "last line $lnm is $line"
      if [ "$line" != "" ];then
        check_config_line $lnm $line
      fi
    fi

    if [[ "$BUILD_OPT" =~ "cak_production" ]]; then
      echo "cak_production with usb plugged, force to set all logs into usb automatically"
      echo "i.e. USBLOG / KERNELUSBLOG / GDLUSBLOG / DIAGBINUSBLOG"
      USBLOG="YES"
      KERNELUSBLOG="YES"
      GDLUSBLOG="YES"
      USBCOREDUMP="YES"
      DIAGBINUSBLOG="YES"
    fi

    USB_AVAILABLE_SPACE=0
    USB_AVAILABLE_SPACE=`df /mnt/usb | grep ${sd_usb}1 | awk '{print $4}'`
    if [ $? -ne 0 ]; then
      USB_AVAILABLE_SPACE=0
    fi
    echo "check USB DISK available space: $USB_AVAILABLE_SPACE"

    # check /mnt/usb/NDS/config/*.cfg
    if [ -d /mnt/usb/NDS/config ];then
      dos2unix /mnt/usb/NDS/config/*
      echo "copy /mnt/usb/NDS/config/* to /NDS/config/"
      cp -rf /mnt/usb/NDS/config/* /NDS/config/
      # Protection against script overwritting.
      echo "delete possible script"
      rm /NDS/config/*.sh
    fi

    # Replace resources present in /mnt/usb/NDS
    if [ -d /mnt/usb/NDS/resources ];then
      echo "epg resources found, replacing them"
      chmod -R 777 /mnt/usb/NDS/resources/
      rm -rf /NDS/resources/*
      cp -rf /mnt/usb/NDS/resources/* /NDS/resources/
    fi

    # Replace metrological present in /mnt/usb/metrological
    if [ -d /mnt/usb/metrological ];then
      echo "metrological resources found, replacing them"
      rm -rf /NDS/metrological/*
      cp -rfp /mnt/usb/metrological/* /NDS/metrological/
    fi

    mkdir -p /mnt/usb/log
    manage_usb_logs

    umount /mnt/usb/

  fi
}

#-----------------------------------------------------------------------------------------------------
# Generate binary config files.
# WARNING: Change working directory into config dir is necessary, because this stupid FusionConfigImport
#          tool index the whole path in the binary file. So if we stay where we are, the files in the
#          binary produced are referenced as "./config/xxx.cfg". And the result is nobody can open any
#          config file. In this context, I really don't understand why the tool takes a path argument,
#          it is useless and confusing...
#-----------------------------------------------------------------------------------------------------
binary_cfg_file_fct() {
echo "-------------- binary config file generation ------------"
current_dir=$(pwd)
cd /NDS/config
if [ "$APPLICATION" = "FATF" ]; then
    /NDS/bin/FusionConfigImport CFG2BIN ./ *.[cC][fF][gG] *.dat
else
    /NDS/bin/FusionConfigImport CFG2BIN ./ *.[cC][fF][gG]
fi
cd $current_dir
}

#-----------------------------------------------------------------------------------------------------
# Core dump Management
# (NOTE: in diag_binary mode, the core dump will be set to Diag binary NFS server)
#-----------------------------------------------------------------------------------------------------
core_dump_fct() {
  enabled=$1

  if [ "${enabled}" == "no" ]; then
    echo "----- Core dump management disabled ----------------------"
    ulimit -c 0
  else
    echo "----- Core dump management -------------------------------"
    if [[ "$BUILD_OPT" =~ "plt_samgw" || "$BUILD_OPT" =~ "plt_samipc_gw" || "$BUILD_OPT" =~ "plt_ciscogw" ]] ; then
      CORE_PATTERN="/mnt/nds/dev_4/part_1/core.%p"
      DEV_STORE="/dev/sda2"
    fi
    if [[ "$BUILD_OPT" =~ "plt_samistb" || "$BUILD_OPT" =~ "plt_samipc" ]] ; then
      CORE_PATTERN="/mnt/nds/dev_2/part_0/core.%p"
      DEV_STORE="/dev/mmcblk0p12"
    fi

    if [ "$USBCOREDUMP" == "YES" ]; then
      if [ $USB_AVAILABLE_SPACE -gt 300000 ];then
        CORE_PATTERN=$USBLOGROOT"/core.%p"
        echo "core dump on usb: $CORE_PATTERN"
      else
        echo "usb available space less than 300MB, don't set core dump on usb"
      fi
    fi

    ulimit -c unlimited

    if [ "$EXECFROM" == "FLASH" ]
    then
      # store COREDUMP on HDD
      echo $CORE_PATTERN > /proc/sys/kernel/core_pattern
    fi

    # create CORE_DIR if not present
    [ -d $CORE_DIR ] || mkdir -p $CORE_DIR
    echo "mount $CORE_DIR"
    # mount $DEV_STORE  into COREDIR
    mount -t ext3  $DEV_STORE $CORE_DIR
    echo "Change working directory to $CORE_DIR"
    cd $CORE_DIR

    if [[ `ls -al core* | wc -l` -ne 0 ]]
    then
      echo "===================================================================="
      echo "A Core dump is present in $CORE_DIR."
      echo "To preserve it, your STB will not start."
      echo "If you don't care about these Core dumps and want to start your STB,"
      echo "please delete all core.* files in $CORE_DIR and restart your STB."
      echo "===================================================================="
      exit
    else
      echo "unmount ext3 in $CORE_DIR"
      umount -l $CORE_DIR
    fi
  fi
}

#-----------------------------------------------------------------------------------------------------
# GDL Memory Monitoring
#-----------------------------------------------------------------------------------------------------
gdl_mem_monitor() {
if [ "$GDLDBG_PERIOD" == "" ]; then
  GDLDBG_PERIOD=5
fi

if [ $GDLDBG_PERIOD -lt 3 ]; then
  GDLDBG_PERIOD=3
fi

if [ "$GDLUSBLOG" == "YES" ] && [ "$USB_PRESENCE" == "YES" ]; then
  GDLUSBLOG="YES"
else
  GDLUSBLOG="NO"
fi

if [  "$GDL_DBG" == "ON" ] || [ "$GDL_LOG_FILE" != "" ] ; then
echo "-------------------- GDL Memory Monitoring (every $GDLDBG_PERIOD seconds) ---------------"
(while ((1)) ; do
 echo /tmp/gdl_mem.txt > /proc/gdl/memory 2>/dev/null
 result=`touch "$GDL_LOG_FILE"`
 if [ $? -eq 0 ]; then
   cat /tmp/gdl_mem.txt | sed -n '/^width.*/{N;N;N;s/^[[:alnum:] _]*: \([[:digit:]]*\)\n[[:alnum:] _]*: \([[:digit:]]*\)\n[[:alnum:] _]*: \([[:digit:]]*\)\n.*/\1x\2 \3/g;P}' | awk '{ SUM += $2;frequences[$0]++} END { printf ";%s;%f",strftime("%s"),SUM/1024/1024; for (n in frequences) {printf ";%s:%d",n,frequences[n];} ;print "\n"}' >> $GDL_LOG_FILE;
 else
   cat /tmp/gdl_mem.txt | sed -n '/^width.*/{N;N;N;s/^[[:alnum:] _]*: \([[:digit:]]*\)\n[[:alnum:] _]*: \([[:digit:]]*\)\n[[:alnum:] _]*: \([[:digit:]]*\)\n.*/\1x\2 \3/g;P}' | awk '{ SUM += $2;frequences[$0]++} END { printf ";%s;%f",strftime("%s"),SUM/1024/1024; for (n in frequences) {printf ";%s:%d",n,frequences[n];} ;print "\n"}';
 fi
 sleep $GDLDBG_PERIOD;
 done )&
fi
}

#-----------------------------------------------------------------------------------------------------
# Diag Binary Management
#-----------------------------------------------------------------------------------------------------
diag_binary_fct() {
BUILD_OPT=$(cat /NDS/options_start)

if [[ ! -f /NDS/DiagBinary.sh ]]  || [[ ! "$BUILD_OPT" =~ "diag_binary" ]];then
  echo "not DiagBinary mode"
  return
fi
# BEGIN DiagBinary.sh
echo "------------- Diag binary management ------------"
DIAGBIN_USE="NO"
if [ "$DIAGBINUSBLOG" = "YES" ] && [ "$NEEDMOUNTUSB" = "YES" ]; then
  echo "diag binary logs on USB $USBLOGROOT"
  DIAGBIN_USE="YES"
  return
fi
if [ "$CAK_PRODUCTION" = "YES" ]; then
  echo "cak production, don't use NFS diag binary server"
  if [[ "$BUILD_OPT" =~ "plt_samgw" || "$BUILD_OPT" =~ "plt_samipc_gw" || "$BUILD_OPT" =~ "plt_ciscogw" ]] ; then
    echo "set size limit to 500MB for GW and IPC_GW"
    sed -i 's/ROTATION_SIZE_LIMIT=.*/ROTATION_SIZE_LIMIT=500715200/g' /NDS/config/diag_binary.cfg
  fi
  if [[ "$BUILD_OPT" =~ "plt_samistb" || "$BUILD_OPT" =~ "plt_samipc" ]] ; then
    echo "set size limit to 300MB for iSTB and IPC"
    sed -i 's/ROTATION_SIZE_LIMIT=.*/ROTATION_SIZE_LIMIT=300715200/g' /NDS/config/diag_binary.cfg
  fi
  return
fi

##-----START--Diag binary: To change the IP of NFS server depending on the site-----
##Initializing the value to IL IP
Diag_binary_host=10.62.14.247;

box_site=$(oneconfig.exe site|cut -d ":" -f 2|tr -d ' ')
echo "----Start load diag Binary configuration(site $box_site)-----"
case "$box_site" in
#Israel Setting
                    1)
                        echo "-------- ISRAEL CONFIGURATION (Site: $box_site) --------"
                        Diag_binary_host=10.62.14.247;
                        ;;
#India Setting
                    2)
                        echo "-------- INDIA CONFIGURATION (Site: $box_site) --------"
                        Diag_binary_host=10.201.7.178;
                        ;;
#France Setting
                    3)
                        echo "-------- FRANCE CONFIGURATION (Site: $box_site) --------"
                        Diag_binary_host=172.21.244.14;
                        Diag_binary_host_path="/local2/upclogs/"
                        Diag_binary_test_conn="NO"
                        Diag_binary_test_port=2049
                        ;;

#India Fr Line Setting
                    4)
                        echo "-------- INDIA FR LINE CONFIGURATION (Site: $box_site) --------"
                        Diag_binary_host=10.201.96.24;
                        Diag_binary_test_conn="NO"
                        Diag_binary_test_port=2049
                        ;;

#default Setting
                    *)
                        echo "-------- DEFAULT (ISRAEL) CONFIGURATION (Site: $box_site) --------"
                        Diag_binary_host=10.62.14.247;
                        ;;
esac

#check the definitions loaded from the USB config, DIAG_BIN_HOST should be the one to active other options
if [ "$DIAG_BIN_HOST" != "" ]; then
  Diag_binary_host="$DIAG_BIN_HOST"

  if [ "$DIAG_BIN_HOST_PATH" != "" ]; then
    Diag_binary_host_path="$DIAG_BIN_HOST_PATH"
  fi

  if [ "$DIAG_BIN_TEST_CONN" = "NO" ]; then
    Diag_binary_test_conn="NO"
  fi

  if [ "$DIAG_BIN_TEST_PORT" != "" ]; then
    Diag_binary_test_port=$DIAG_BIN_TEST_PORT
  fi
fi

echo "Diag_binary_host: $Diag_binary_host"
echo "Diag_bin_host_path: $Diag_binary_host_path"
echo "Diag_binary_test_conn: $Diag_binary_test_conn"
echo "Diag_binary_test_port: $Diag_binary_test_port"
echo "----END-Diag Binary configuration-----"

##-----END--Diag binary: To change the IP of the NFS server depending on the site-----

echo "--------- mount NFS server for diagbinary ---------"
  if [ "$Diag_binary_host_path" != "" ]; then opt_host_path="host_path=$Diag_binary_host_path"; fi
  if [ "$Diag_binary_test_conn" != "" ]; then opt_test_conn="test_conn=$Diag_binary_test_conn"; fi
  if [ "$Diag_binary_test_port" != "" ]; then opt_test_port="test_port=$Diag_binary_test_port"; fi
  source /NDS/DiagBinary.sh 1 $Diag_binary_host "$opt_host_path" $opt_test_conn $opt_test_port
sed -i 's/DIR_LOG_PATH=.*/DIR_LOG_PATH="\/binary_logs"/g' /NDS/config/diag_binary.cfg
  DIAGBIN_USE="YES";
echo "set core dump path to /binary_logs/core-%t"
echo /binary_logs/core-%t > /proc/sys/kernel/core_pattern
}

#-----------------------------------------------------
# NGI Auto Mock
#-----------------------------------------------------
ngi_auto_mock_fct() {
# ngi_overview.json
if [[ "$BUILD_OPT" =~ "plt_samipc" ]]
then
  cp -f /NDS/config/ngi_overview_IPC_v4_v6.json /NDS/config/ngi_overview.json
else
  udhcpc -q -i $ETHINTER -V STB-UPC-Int -s /etc/udhcpc.script
  cp -f /NDS/config/ngi_overview_v4_v6.json /NDS/config/ngi_overview.json
fi

# ngi_cable_link.json
cp -f /NDS/config/ngi_cable_link_v4_v6.json /NDS/config/ngi_cable_link.json

# ngi_wanipconnection.json and ngi_wanipv6firewall.json
if [ "$NGI_MOCK_WANIPV4" == "YES" ]
then
  /NDS/automock_ngi.sh inet $ETHINTER /NDS/config/ngi_wanipconnection_v4.json
  cp -f /NDS/config/ngi_wanipconnection_v4.json /NDS/config/ngi_wanipconnection.json
  cp -f /NDS/config/ngi_wanipv6firewall_v4.json /NDS/config/ngi_wanipv6firewall.json
fi
if [ "$NGI_MOCK_WANIPV6" == "YES" ]
then
  /NDS/automock_ngi.sh inet6 $ETHINTER /NDS/config/ngi_wanipconnection_v6.json
  cp -f /NDS/config/ngi_wanipconnection_v6.json /NDS/config/ngi_wanipconnection.json
  cp -f /NDS/config/ngi_wanipv6firewall_v6.json /NDS/config/ngi_wanipv6firewall.json
fi
}

#-----------------------------------------------------------------------------------------------------
# NFS Adaptation
#-----------------------------------------------------
#                    |        routes                 |
#-----------------------------------------------------
#                    | gdb   | telnet | diag | nfs   |
# gdb nfs            |  O    |   O    |      |  O    |
# gdb flash          |  O    |   O    |      |       |
# nfs                |       |   O    |  O   |  O    |
# telnet start nfs   |       |   O    |  O   |  O    |
# telnet start flash |       |   O    |  O   |       |
#-----------------------------------------------------
# This next block is our first attempt at adding routes to support removal of the default gw
# when NCM starts up - and is intended to ensure the MW still works effectively in the test
# environment
# NOTE: IPv6 is not yet supported.
#-----------------------------------------------------------------------------------------------------
nfs_gdb_adatptation_fct() {
if [[ "$EXECFROM" == "NFS" || "$ISGDBON" == "YES" || "$TELNET_START" == "YES" ]]
then

# NFS setup
[ "$EXECFROM" == "NFS" ] && chmod +x *

# GDB setup
if [ "$ISGDBON" == "YES" ]
then
  echo "Adding GDB firewall rule"
  iptables -A INPUT -p tcp --dport $GDB_PORT -j ACCEPT

  if [ "$EXECFROM" == "FLASH" ]
  then
    if [ $ISDOCSISBUILD == "YES" ]
    then
      echo "wait DOCSIS 1 minute"
      i=0; while (( i '<' 60 )); do echo -e '*\ci' ;i=$i+1 ;  sleep 1; done
    fi
    echo "get IP Address of iface $ETHINTER"
    if [[ "$BUILD_OPT" =~ "plt_samipc" ]]
    then
      udhcpc -q -i $ETHINTER -V STB-UPC-Ext -C -s /etc/udhcpc.script
    else
      udhcpc -q -i $ETHINTER -V STB-UPC-Int -s /etc/udhcpc.script
    fi
  fi
  route -n
fi

# Route management
    ROUTE_CMD=`which route`
    MOUNT_CMD=`which mount`
  echo "--------------------- nfs/gdb adaptation --------------------"
    echo "modify NCM keys..."
# ncm.cfg
    sed  "s/^\(DEFAULT_LAN_INTERFACE=\).*$/\1$ETHINTER/" -i /NDS/config/ncm.cfg
  if [[ "$DROOTFS" == "NFS" || "$BUILD_OPT" =~ "plt_samipc_gw" || "$BUILD_OPT" =~ "plt_samipc" ]]

  then

    sed  '/INTERFACE_1/,/INTERFACE_2/ s/^\(IS_IGNORED=\).*$/\1FALSE/' -i /NDS/config/ncm.cfg
    sed  '/INTERFACE_1/,/INTERFACE_2/ s/^\(IS_HIDDEN=\).*$/\1FALSE/' -i /NDS/config/ncm.cfg
    sed  '/INTERFACE_1/,/INTERFACE_2/ s/^\(IS_DEFAULT_LAN_INTERFACE=\).*$/\1TRUE/' -i /NDS/config/ncm.cfg
    sed  '/INTERFACE_4/,$ s/^\(IS_DEFAULT_LAN_INTERFACE=\).*$/\1FALSE/' -i /NDS/config/ncm.cfg
    if [[ "$BUILD_OPT" =~ "plt_samipc_gw" || "$BUILD_OPT" =~ "plt_samipc" ]]
    then
        echo "ULive01496489  : [Uh356] [UPC IPC] Interface changes in UPC IPC Box, requiring NCM config changes"
    else
        sed  '/INTERFACE_4/,$ s/^\(IS_IGNORED=\).*$/\1TRUE/' -i /NDS/config/ncm.cfg
        sed  '/INTERFACE_4/,$ s/^\(IS_HIDDEN=\).*$/\1TRUE/' -i /NDS/config/ncm.cfg
    fi


  fi
# ncm_default_factory.cfg
if [[ "$BUILD_OPT" =~ "plt_samgw"  || "$BUILD_OPT" =~ "plt_samistb" ]] ; then
  sed  's/^\(KEEP_ALL_BOOT_ROUTE=\).*$/\1TRUE/' -i /NDS/config/ncm_default_factory.cfg
  sed  '/INTERFACE1/,/INTERFACE2/ s/^\(FLAGS=\).*$/\10x8043/'  -i /NDS/config/ncm_default_factory.cfg
  sed  "/INTERFACE1/,/INTERFACE2/ s/^\(NAME=\).*$/\1$ETHINTER/" -i /NDS/config/ncm_default_factory.cfg
  sed  's/^\(KEEP_IPV4_ADDRESS=\).*$/\1TRUE/' -i /NDS/config/ncm_default_factory.cfg
  sed  's/^\(NB_INTERFACE=\).*$/\12/' -i /NDS/config/ncm_default_factory.cfg
fi
if [[ "$BUILD_OPT" =~ "plt_ciscogw" ]] ; then
  sed  '/INTERFACE1/,/INTERFACE2/ s/^\(FLAGS=\).*$/\10x8043/'  -i /NDS/config/ncm_default_factory.cfg
  sed  "/INTERFACE1/,/INTERFACE2/ s/^\(NAME=\).*$/\1$ETHINTER/" -i /NDS/config/ncm_default_factory.cfg
fi
  echo "...done "

# For IPC only
if [[ "$BUILD_OPT" =~ "plt_samipc_gw" || "$BUILD_OPT" =~ "plt_samipc" ]] ; then
  if [  "$ISMOUNT" == "NO" ] ; then
    echo "Getting the IP First"
    iwpriv ce00 set CountryRegionABand=1
    rm -f /tmp/cm_lan_mac && udhcpc -q -i $ETHINTER -n -s /etc/udhcpc.script
    sed -i 's/UTC_SERVER_URI=".*"/UTC_SERVER_URI="192.168.192.2"/' /NDS/config/spm.cfg
  fi
fi

    echo "get ipaddress/mask..."
    NFS_IP=`ifconfig | sed -n "/^$ETHINTER /,/inet / s/^.*addr:\(.*\) *Bcast.*/\1/p"`
    NFS_MASK=`ifconfig | sed -n "/^$ETHINTER /,/inet / s/^.*Mask:\(.*\)$/\1/p"`
    ((NFS_IP_MSKED_1=`echo $NFS_IP | cut -d . -f 1`&`echo $NFS_MASK  | cut -d . -f 1`))
    ((NFS_IP_MSKED_2=`echo $NFS_IP | cut -d . -f 2`&`echo $NFS_MASK  | cut -d . -f 2`))
    ((NFS_IP_MSKED_3=`echo $NFS_IP | cut -d . -f 3`&`echo $NFS_MASK  | cut -d . -f 3`))
    ((NFS_IP_MSKED_4=`echo $NFS_IP | cut -d . -f 4`&`echo $NFS_MASK  | cut -d . -f 4`))
    NFS_IP_MSKED=$NFS_IP_MSKED_1.$NFS_IP_MSKED_2.$NFS_IP_MSKED_3.$NFS_IP_MSKED_4
    echo "ipadress=$NFS_IP ipmask=$NFS_MASK ipaddrmask=$NFS_IP_MSKED"
    echo "...done "

    echo "get gateway address..."
    DEFAULT_IPV4_GATEWAY=`${ROUTE_CMD} -n | grep ^0.0.0.0 | sed -e 's/^0.0.0.0[ ]\+\([^ ]\+\)[ ]\+.*/\1/'`
    echo "gatewayaddress=$DEFAULT_IPV4_GATEWAY"
    echo "...done "

if [ "${DEFAULT_IPV4_GATEWAY}" != "" ]
then
    # Manage all NFS mounts over IP.
    # Have to look for hostname in address as well as the IP address (just in case)
    if [ "$EXECFROM" == "NFS" ]
    then
    echo "manage NFS mounts..."
    for NFS_SRV_IP in `${MOUNT_CMD} | sed -n 's/.*addr=\([0-9\.a-zA-Z]\+\))/\1/p' | uniq`
    do
        ((NFS_SRV_IP_MSKED_1=`echo $NFS_SRV_IP | cut -d . -f 1`&`echo $NFS_MASK  | cut -d . -f 1`))
        ((NFS_SRV_IP_MSKED_2=`echo $NFS_SRV_IP | cut -d . -f 2`&`echo $NFS_MASK  | cut -d . -f 2`))
        ((NFS_SRV_IP_MSKED_3=`echo $NFS_SRV_IP | cut -d . -f 3`&`echo $NFS_MASK  | cut -d . -f 3`))
        ((NFS_SRV_IP_MSKED_4=`echo $NFS_SRV_IP | cut -d . -f 4`&`echo $NFS_MASK  | cut -d . -f 4`))
        NFS_SRV_IP_MSKED=$NFS_SRV_IP_MSKED_1.$NFS_SRV_IP_MSKED_2.$NFS_SRV_IP_MSKED_3.$NFS_SRV_IP_MSKED_4
        echo "ipsrvadress=$NFS_SRV_IP  ipsrvaddrmask=$NFS_SRV_IP_MSKED"
        if [ "$NFS_SRV_IP_MSKED" = "$NFS_IP_MSKED" ]
        then
           echo "sub network is the same, don't add route"
        else
           CMDS="${ROUTE_CMD} add -host ${NFS_SRV_IP} gw ${DEFAULT_IPV4_GATEWAY}"
           echo "Add a route for NFS mount: ${CMDS}"
           ${CMDS}
        fi
    done
    echo "...done "
    fi

    # Manage the DIAG_OUTPUT if IP is used.  Straightforward . . .
    if [ "$EXECFROM" == "NFS" ]
    then
    echo "manage diag..."
    DIAG_IPV4_ADDR=`sed -n 's/^.*host=\(.*\);\r*$/\1/p' < config/diagctl.cfg`
    if [ $DIAG_IPV4_ADDR ]
    then
       DIAG_DEST=`sed -n 's/^.*protocol=\(.*\);\r*$/\1/p' < config/diagctl.cfg`
       DIAG_PORT=`sed -n 's/^.*port=\(.*\);\r*$/\1/p' < config/diagctl.cfg`
       ((NFS_SRV_IP_MSKED_1=`echo $DIAG_IPV4_ADDR | cut -d . -f 1`&`echo $NFS_MASK  | cut -d . -f 1`))
       ((NFS_SRV_IP_MSKED_2=`echo $DIAG_IPV4_ADDR | cut -d . -f 2`&`echo $NFS_MASK  | cut -d . -f 2`))
       ((NFS_SRV_IP_MSKED_3=`echo $DIAG_IPV4_ADDR | cut -d . -f 3`&`echo $NFS_MASK  | cut -d . -f 3`))
       ((NFS_SRV_IP_MSKED_4=`echo $DIAG_IPV4_ADDR | cut -d . -f 4`&`echo $NFS_MASK  | cut -d . -f 4`))
       NFS_SRV_IP_MSKED=$NFS_SRV_IP_MSKED_1.$NFS_SRV_IP_MSKED_2.$NFS_SRV_IP_MSKED_3.$NFS_SRV_IP_MSKED_4
       echo "ipsrvadress=$NFS_SRV_IP  ipsrvaddrmask=$NFS_SRV_IP_MSKED"
       if [ "$NFS_SRV_IP_MSKED" = "$NFS_IP_MSKED" ]
       then
          echo "sub network is the same, don't add route"
       else
          if [ "${DIAG_DEST}" == "udp" -o "${DIAG_DEST}" == "tcp" ]
          then
              CMDS="${ROUTE_CMD} add -host ${DIAG_IPV4_ADDR} gw ${DEFAULT_IPV4_GATEWAY}"
              echo "Add a route for DIAG: ${CMDS}"
              ${CMDS}
          fi
       fi
    else
       echo "NO DIAG detected"
    fi
    echo "...done "
    fi

    # Telnet over TCPIP (on Product Platforms)
    echo "manage telnet..."
    if [ -e "/proc/net/tcp" ]
    then
        for hex_telnet_addr in `awk -F ' '  'NR != 1 {printf $2 " " $3 "\n"}' /proc/net/tcp | grep ':0017' | grep -v ':0000' | awk -F ' ' '{printf $2 "\n"}' | cut -f 1 -d':'`
        do
            ip_address=`echo $hex_telnet_addr | awk '{FS=""; printf "%d.%d.%d.%d","0x"$7$8,"0x"$5$6,"0x"$3$4,"0x"$1$2}' -`
            ((NFS_SRV_IP_MSKED_1=`echo $ip_address | cut -d . -f 1`&`echo $NFS_MASK  | cut -d . -f 1`))
            ((NFS_SRV_IP_MSKED_2=`echo $ip_address | cut -d . -f 2`&`echo $NFS_MASK  | cut -d . -f 2`))
            ((NFS_SRV_IP_MSKED_3=`echo $ip_address | cut -d . -f 3`&`echo $NFS_MASK  | cut -d . -f 3`))
            ((NFS_SRV_IP_MSKED_4=`echo $ip_address | cut -d . -f 4`&`echo $NFS_MASK  | cut -d . -f 4`))
            NFS_SRV_IP_MSKED=$NFS_SRV_IP_MSKED_1.$NFS_SRV_IP_MSKED_2.$NFS_SRV_IP_MSKED_3.$NFS_SRV_IP_MSKED_4
            echo "ipsrvadress=$NFS_SRV_IP  ipsrvaddrmask=$NFS_SRV_IP_MSKED"
            if [ "$NFS_SRV_IP_MSKED" = "$NFS_IP_MSKED" ]
            then
               echo "sub network is the same, don't add route"
            else
               CMDS="${ROUTE_CMD} add -host ${ip_address} gw ${DEFAULT_IPV4_GATEWAY}"
               echo "Add a route for Telnet over TCPIP: ${CMDS}"
               ${CMDS}
            fi
        done
    fi
    if [ -e "/proc/net/tcp6" ]
    then
        for hex_telnet_addr in `awk -F ' '  'NR != 1 {printf $2 " " $3 "\n"}' /proc/net/tcp6 | grep ':0017' | grep -v ':0000' | awk -F ' ' '{printf substr($2,25,8) "\n"}' | cut -f 1 -d':'`
        do
            ip_address=`echo $hex_telnet_addr | awk '{FS=""; printf "%d.%d.%d.%d","0x"$7$8,"0x"$5$6,"0x"$3$4,"0x"$1$2}' -`
            ((NFS_SRV_IP_MSKED_1=`echo $ip_address | cut -d . -f 1`&`echo $NFS_MASK  | cut -d . -f 1`))
            ((NFS_SRV_IP_MSKED_2=`echo $ip_address | cut -d . -f 2`&`echo $NFS_MASK  | cut -d . -f 2`))
            ((NFS_SRV_IP_MSKED_3=`echo $ip_address | cut -d . -f 3`&`echo $NFS_MASK  | cut -d . -f 3`))
            ((NFS_SRV_IP_MSKED_4=`echo $ip_address | cut -d . -f 4`&`echo $NFS_MASK  | cut -d . -f 4`))
            NFS_SRV_IP_MSKED=$NFS_SRV_IP_MSKED_1.$NFS_SRV_IP_MSKED_2.$NFS_SRV_IP_MSKED_3.$NFS_SRV_IP_MSKED_4
            echo "ipsrvadress=$NFS_SRV_IP  ipsrvaddrmask=$NFS_SRV_IP_MSKED"
            if [ "$NFS_SRV_IP_MSKED" = "$NFS_IP_MSKED" ]
            then
               echo "sub network is the same, don't add route"
    else
               CMDS="${ROUTE_CMD} add -host ${ip_address} gw ${DEFAULT_IPV4_GATEWAY}"
               echo "Add a route for Telnet over TCPIP: ${CMDS}"
               ${CMDS}
            fi
        done
    fi

    if [ ! -e "/proc/net/tcp" -a ! -e "/proc/net/tcp6" ]
    then
       echo "No telnet detected"
    fi
    echo "manage done..."

    # Manage GDB debug route
    if [ "$ISGDBON" == "YES" ]
    then
      if [ $GDB_CLIENT_ADDR ]
      then
         ((NFS_SRV_IP_MSKED_1=`echo $GDB_CLIENT_ADDR | cut -d . -f 1`&`echo $NFS_MASK  | cut -d . -f 1`))
         ((NFS_SRV_IP_MSKED_2=`echo $GDB_CLIENT_ADDR | cut -d . -f 2`&`echo $NFS_MASK  | cut -d . -f 2`))
         ((NFS_SRV_IP_MSKED_3=`echo $GDB_CLIENT_ADDR | cut -d . -f 3`&`echo $NFS_MASK  | cut -d . -f 3`))
         ((NFS_SRV_IP_MSKED_4=`echo $GDB_CLIENT_ADDR | cut -d . -f 4`&`echo $NFS_MASK  | cut -d . -f 4`))
         NFS_SRV_IP_MSKED=$NFS_SRV_IP_MSKED_1.$NFS_SRV_IP_MSKED_2.$NFS_SRV_IP_MSKED_3.$NFS_SRV_IP_MSKED_4
         echo "ipsrvadress=$NFS_SRV_IP  ipsrvaddrmask=$NFS_SRV_IP_MSKED"
         if [ "$NFS_SRV_IP_MSKED" = "$NFS_IP_MSKED" ]
         then
            echo "sub network is the same, don't add route"
         else
            CMDS="${ROUTE_CMD} add -host ${GDB_CLIENT_ADDR} gw ${DEFAULT_IPV4_GATEWAY}"
            echo "Add a route for GDB: ${CMDS}"
            ${CMDS}
         fi
      else
         echo "No gdb client address defined"
      fi
      echo "...done "
    fi

    if [ -e "/proc/net/tcp" ]
    then
      echo "***************************************************************"
      echo " IPv4 routing table"
      echo "***************************************************************"
      ${ROUTE_CMD}
      echo "***************************************************************"
    fi

    if [ -e "/proc/net/tcp6" ]
    then
      echo "***************************************************************"
      echo " IPv6 routing table"
      echo "***************************************************************"
      ${ROUTE_CMD} -A inet6
      echo "***************************************************************"
    fi
fi
fi
}


####################
### ENVIRONEMENT ###
####################
HOMEDIR=`pwd`
BIN_DIR="/NDS/bin"
CORE_DIR="/corefile"
APPLICATION="EPG"
ONECONFIG=`which oneconfig.exe`
XBMC="ONDEMAND"
GDB_PORT=2159

LOG_FILE=""
USBLOG="NO"

# USB support for Prod/Rdiag/diagbin builds - CISCO platform only
MSDOS_MODULE=/NDS/drivers/msdos.ko
FAT_MODULE=/NDS/drivers/fat.ko
VFAT_MODULE=/NDS/drivers/vfat.ko
if [ -e $MSDOS_MODULE ] && [ -e $FAT_MODULE ] && [ -e $VFAT_MODULE ] ; then
  echo "------------------- inserting usb drivers - H2S --------------------"
  insmod $FAT_MODULE
  insmod $VFAT_MODULE
  insmod $MSDOS_MODULE
  rm -f $FAT_MODULE
  rm -f $VFAT_MODULE
  rm -f $MSDOS_MODULE
else
  echo "------------------- usb drivers are missing - H2S --------------------"
fi

# Check USB config
check_usb_config_fct

#Driver in rootfs interface
HARD_MODEL=$(cat /dev/nds/versioninformation | sed -n s/^MODEL_NAME://p | cut -c1-9)
# iSTB
if [ $HARD_MODEL == "SMT-C5400" ] ; then
  cat /proc/cmdline | grep "nfsroot" > /dev/null; if [ $? == 0 ]; then  DROOTFS="NFS"; ETHINTER="br0"; else DROOTFS="RAM"; ETHINTER="br0";fi
# IPClient
elif [ $HARD_MODEL == "SMT-E6400" ] ; then
  cat /proc/cmdline | grep "nfsroot" > /dev/null; if [ $? == 0 ]; then  DROOTFS="NFS"; ETHINTER="eth0"; else DROOTFS="RAM"; ETHINTER="eth0";fi
# Refresh Gateway
elif [ $HARD_MODEL == "SMT-G7401" ] ; then
  sed  's/^\(devices_sector_size=\).*$/devices_sector_size=2/' -i /NDS/config/pdm_fs.cfg
  cat /proc/cmdline | grep "nfsroot" > /dev/null; if [ $? == 0 ]; then  DROOTFS="NFS"; ETHINTER="eth0"; else DROOTFS="RAM"; ETHINTER="br0";fi
elif [ $HARD_MODEL == "UGW1726" ] ; then
  sed  's/^\(devices_sector_size=\).*$/devices_sector_size=2/' -i /NDS/config/pdm_fs.cfg
  cat /proc/cmdline | grep "nfsroot" > /dev/null; if [ $? == 0 ]; then  DROOTFS="NFS"; ETHINTER="eth0"; else DROOTFS="RAM"; ETHINTER="eth0";fi
# Gateway ($HARD_MODEL == "SMT-G7400")
else
  cat /proc/cmdline | grep "nfsroot" > /dev/null; if [ $? == 0 ]; then  DROOTFS="NFS"; ETHINTER="eth0"; else DROOTFS="RAM"; ETHINTER="br0";fi
fi

mount | grep "type nfs" > /dev/null; if [ $? == 0 ];then  ISMOUNT="YES"; else ISMOUNT="NO"; fi
mount | awk '/type nfs/ {print $3}' | grep `pwd` >/dev/null ;  if [ $? == 0 ]; then export EXECFROM="NFS" ; else export EXECFROM="FLASH" ; fi
if [ -f $BIN_DIR/SCD_Process ]; then SCDPROCESS="YES"; else SCDPROCESS="NO"; fi
if [ "$SCDPROCESS" == "NO" ] ; then PERFORMMOUNT="YES"; elif [ "$EXECFROM" == "NFS" -a "$DROOTFS" == "RAM" ]; then PERFORMMOUNT="YES"; else  PERFORMMOUNT="NO"; fi
if [ "$1" == "GDB" ] ; then ISGDBON="YES"; else ISGDBON="NO" ; fi
if [ "$2" ] ; then GDB_CLIENT_ADDR="$2"; else GDB_CLIENT_ADDR="NOT_DEFINED" ; fi
IS_FORMAT_DETECTED="NO" ; for arg in $* ; do [ "$arg" == "FORMAT" ] && IS_FORMAT_DETECTED="YES" ; done;
IS_XBMC_CMDLINE_DETECTED="NO" ; for arg in $* ; do [ "$arg" == "XBMC" ] && IS_XBMC_CMDLINE_DETECTED="YES" ; done;

CI_TARGET=`echo $* | awk -F' ' '{print $NF}'`

if [ "$PERFORMMOUNT" == "YES" ] ; then BUILD_OPT=$(cat $HOMEDIR/options_start) ; else BUILD_OPT=$(cat /NDS/options_start) ; fi
# telnet start
netstat -n 2>/dev/null | grep ":23" >/dev/null ; if [ $? == 0 ]; then TELNET_START="YES" ; else TELNET_START="NO" ; fi

# build option management
if [[ "$BUILD_OPT" =~ "docsis" ]] ; then ISDOCSISBUILD="YES"; else ISDOCSISBUILD="NO" ; fi
if [[ "$BUILD_OPT" =~ "ngi_mock_wanipv4" ]] ; then NGI_MOCK_WANIPV4="YES"; else NGI_MOCK_WANIPV4="NO" ; fi
if [[ "$BUILD_OPT" =~ "ngi_mock_wanipv6" ]] ; then NGI_MOCK_WANIPV6="YES"; else NGI_MOCK_WANIPV6="NO" ; fi
if [[ "$BUILD_OPT" =~ "cak_production" ]] ; then CAK_PRODUCTION="YES" ; else CAK_PRODUCTION="NO"; fi
if [[ "$BUILD_OPT" =~ "diag_binary" ]] ; then DIAG_BINARY_MODE="YES" ; else DIAG_BINARY_MODE="NO"; fi
if [[ "$BUILD_OPT" =~ "force_xbmc" ]] ; then XBMC="ON" ; fi
if [[ "$BUILD_OPT" =~ "tcpdump" ]] ; then ISTCPDUMP="YES"; else ISTCPDUMP="NO" ; fi
if [ "$#" = "0" ];then UNIFIED_ARGS="NOCLEAN";else UNIFIED_ARGS=$*; fi

#GDL debug
if [[ $(oneconfig.exe gdl_dbg) =~ "ON" ]] ; then GDL_DBG="ON" ; else GDL_DBG="OFF" ; fi

#Disabeling portal autostart for FUAPI purpose
if [[ $(oneconfig.exe metro_portal) =~ "OFF" ]] && [[ ! "$BUILD_OPT" =~ "cak_production" ]] && [[ "$EXECFROM" = "FLASH" ]] ; then METRO_DISABLED="ON" ; else METRO_DISABLED="OFF" ; fi


START_TIMESTAMP=`date +'%s'`
START_UPTIME=`cat /proc/uptime | awk '{print $1}'`
NDS_SW_VERSION=`cat /NDS/config/version.cfg | grep NDS_SW_VERSION | awk -F '=' '{print $2}'`
echo "----------------- ENVIRONEMENT VARIABLES ----------------"
echo "---------------------------------------------------------"
echo "Start time(linux timestamp,uptime): ${START_TIMESTAMP},${START_UPTIME}"
echo "Binary version:           "$NDS_SW_VERSION
echo "Hard model:               "$HARD_MODEL
echo "Drivers Rootfs:           "$DROOTFS
echo "NFS mount present:        "$ISMOUNT
echo "Execution from:           "$EXECFROM
echo "Ethernet interface:       "$ETHINTER
echo "SCD_Process in /NDS:      "$SCDPROCESS
echo "Perform mount on /NDS:    "$PERFORMMOUNT
echo "Unified Args:             "$UNIFIED_ARGS
echo "FATF/CI Target specified: "$CI_TARGET
echo "Is FORMAT param detected: "$IS_FORMAT_DETECTED
echo "Build options detected:   "$BUILD_OPT
echo "XBMC                      "$XBMC
echo "XBMC forced by cmdline    "$IS_XBMC_CMDLINE_DETECTED
echo "Telnet start              "$TELNET_START
echo "NGI mock wanipv4          "$NGI_MOCK_WANIPV4
echo "NGI mock wanipv6          "$NGI_MOCK_WANIPV6
echo "---------------------------------------------------------"
echo "GDL debug                 "$GDL_DBG
echo "Is GDB DEBUG:             "$ISGDBON
[[ $ISGDBON == "YES" ]] && echo "GDB client(pc) addr:      "$GDB_CLIENT_ADDR
echo "Is DOCSIS Binary:         "$ISDOCSISBUILD
echo "Is USBLOG param detected: "$USBLOG
echo "---------------------------------------------------------"

echo "----------------- Checking configuration ----------------"
if [ "$ISDOCSISBUILD" == "YES"  -a  "$DROOTFS" == "NFS" ]; then echo "You are trying to start a DOCSIS Binary with nfs rootfs (using redboot), it's forbidden, you have to perform a onenand boot "; exit; fi
if [ "$EXECFROM" == "FLASH"  -a  "$ISMOUNT" == "YES" ]; then echo "You are trying to start STB excuting this script from flash with NFS mount active, it's forbidden, you have to remove the NFS mount or excute this script from the NFS mount"; exit; fi
if [ "$ISGDBON" == "YES" ]
then
  if [ "$GDB_CLIENT_ADDR" == "NOT_DEFINED" ]; then echo "You are trying to start a gdb session, you need to specify the ip addr of the gdb client: start.sh GDB xxx.xxx.xxx.xxx"; exit; fi
  echo "$2" | grep '^[[:digit:]]\{1,3\}\.[[:digit:]]\{1,3\}\.[[:digit:]]\{1,3\}\.[[:digit:]]\{1,3\}$' > /dev/null
  if [ $? == 1 ]; then echo "You are trying to start a gdb session, the given ip adress doesn't respect the following format  xxx.xxx.xxx.xxx"; exit; fi
  if [ ! -f /usr/local/bin/gdbserver ]; then echo "You are trying to start a gdb session, the gdbserver is missing, please check your drivers"; exit; fi
fi
echo "configuration is OK"


####################
### SCRIPT START ###
####################

#dipslay cpe_id in logs.It helps IEX infrastructure to know where are located the cpe. below command works only for Samsung plft till now
if [[ "$BUILD_OPT" =~ "plt_samgw" || "$BUILD_OPT" =~ "plt_samipc_gw" || "$BUILD_OPT" =~ "plt_samistb" || "$BUILD_OPT" =~ "plt_samipc" ]] ; then
  cpe_id=$(cat /dev/nds/versioninformation |grep PAIRING | cut -b 29-36)
  cpe_id=$(printf "%010d" 0x$cpe_id)
  echo "cpe_id: $cpe_id"
fi

#Perform mount
echo "------------------- mount management --------------------"
if [ "$PERFORMMOUNT" == "YES" ]
then
    echo "Perform mount from $HOMEDIR to /NDS"
    mount ${HOMEDIR} /NDS
fi

#USB Launcher
[[ "$BUILD_OPT" =~ "usb_launcher" ]] && usb_launcher_fct

# bootlog
if [[ -f /NDS/bootlog.sh ]]
then
    echo "------------------- bootlog process ---------------------"
    /NDS/bootlog.sh 1 1600 upload
    cd /
    umount /db_data 2>/dev/null
    umount /bl_data 2>/dev/null
    echo "bootlog completed"
else
    echo "bootlog not found in /NDS/bootlog.sh"
fi

# Core dump management done before we put down ethernet interface. IEX telnet cpe to get core dump
core_dump_fct "yes"

if [ "$ISDOCSISBUILD" == "YES" ]
then
#    echo "PATCH : Remove 'echo 1 > /proc/ipv6_original_ra' from unified_startup.sh"
#    if [ "$PERFORMMOUNT" == "YES" ] ; then BIN_DIR_TEMP="$HOMEDIR" ; else BIN_DIR_TEMP="/NDS" ; fi
#    cp -f $BIN_DIR_TEMP/unified_startup.sh $BIN_DIR_TEMP/unified_startup_temp.sh
#    sed '/ipv6_original_ra/d' $BIN_DIR_TEMP/unified_startup.sh > $BIN_DIR_TEMP/unified_startup_temp.sh && cp -f $BIN_DIR_TEMP/unified_startup_temp.sh $BIN_DIR_TEMP/unified_startup.sh && rm -f $BIN_DIR_TEMP/unified_startup_temp.sh
    if [ "$PERFORMMOUNT" == "NO" -a "$TELNET_START" == "NO" ] ; then echo "PATCH : set interface $ETHINTER to down" && ifconfig $ETHINTER 0.0.0.0 down && rm /tmp/resolv.conf ; fi
    if [[ "$BUILD_OPT" =~ "plt_samipc" ]] ; then if [ "$PERFORMMOUNT" == "NO" -a "$TELNET_START" == "NO" ] ; then echo "PATCH : set wifi interface ce00 to down" && ifconfig ce00 0.0.0.0 down ; fi ; fi
else
    echo "Add WITH_IPV6 option to unified_startup.sh when working with non docsis builds (DMZ for example)"
    UNIFIED_ARGS="$UNIFIED_ARGS WITH_IPV6"
fi

echo "-------------------- unified startup call ---------------"
cd /NDS
./unified_startup.sh /NDS /NDS $UNIFIED_ARGS NORUN NOCLEAN_DB WITHOUT_INSTALL_BUILD

# Until unified_startup.sh is modified: update this as per WP2807
sed -i 's/SCD/PWM/g' /root/run.sh

#  Adding Firewall rules against RA (depending if you're working from PUMA5 or DMZ Ethernet)
if [ "$ISDOCSISBUILD" == "YES" ]
then
    echo "PATCH : Router solicitation Filtering (CM : RA sur adresse multicast standard)"
    #ip6tables -I INPUT 1 -p ipv6-icmp --icmpv6-type 134 --destination ff02::1/128 -j DROP
else
    echo "PATCH : Router solicitation Filtering (DMZ : RA sur adresse multicast samsung)"
    ip6tables -I INPUT 1 -p ipv6-icmp --icmpv6-type 134 --destination ff04::100/128 -j DROP
fi

#inserting drivers for keyboard
HID_MODULE=/NDS/config/hid.ko
USBHID_MODULE=/NDS/config/usbhid.ko
EVDEV_MODULE=/NDS/config/evdev.ko
if [ -e ${HID_MODULE} ] && [ -e ${USBHID_MODULE} ] && [ -e ${EVDEV_MODULE} ] ; then
  echo "------------------- inserting keyboard drivers --------------------"
  insmod ${HID_MODULE}
  insmod ${USBHID_MODULE}
  insmod ${EVDEV_MODULE}
  rm -f  ${HID_MODULE}
  rm -f  ${USBHID_MODULE}
  rm -f  ${EVDEV_MODULE}
else
  echo "------------------- keyboard drivers are missing --------------------"
fi

if [[ "$BUILD_OPT" =~ "WebkitWPE_engine" ]]; then
  if [[ "$BUILD_OPT" =~ "plt_samgw"  || "$BUILD_OPT" =~ "plt_ciscogw" ]]; then
    # clear swap is not NASC compliant!!!
    ./start_swap.sh 512 &
    #./start_swap_dm-crypt.sh 512 &
    #./start_partition_dm-crypt.sh sda1 &
    #./start_swap_partition.sh sda1 &
    #./start_swap_partition_dm-crypt.sh sda1 &
  fi
fi

# TMP TMP
ifconfig eth0:1 down
ifconfig br0:1 down
# TMP TMP

# FATF or EPG
if [[ -f fatf_sync.sh ]]
then
    echo "---------------------- FATF Management ------------------"
    APPLICATION="FATF"
    echo "fatf_sync.sh present, hence syncing"
    cd /NDS
    ./fatf_sync.sh $UNIFIED_ARGS
fi

# bootlog for start.sh FORMAT
if [ -f /NDS/bootlog.sh -a "$IS_FORMAT_DETECTED" == "YES" ]
then
    /NDS/bootlog.sh ONFORMAT
fi

# NGI auto Mock
if [ $NGI_MOCK_WANIPV4 == "YES" -o $NGI_MOCK_WANIPV6 == "YES" ]
then
    ngi_auto_mock_fct
fi

# NFS GDB Adaptation
nfs_gdb_adatptation_fct

# Display some information for debug purpose.
echo "--------------------- Debug Information -----------------"
echo "============ Kernel IP info ...."
cat /etc/resolv.conf
if [ -e "/proc/net/tcp" ]
then
    echo "########## ROUTE :"
    route -n
fi
if [ -e "/proc/net/tcp6" ]
then
    echo "########## ROUTE 6 :"
    route -n -A inet6
fi
echo "########## IFCONFIG -A : "
ifconfig -a
echo "########## mounting points : "
mount
echo "########## PS : "
ps


#patch istb/no xbmc - Temporary Patch for DLNA
[[ "$BUILD_OPT" =~ "plt_samistb" ]] && echo "---- PATCH: ISTB ethtool -C eth0 rx-usecs 5000  (hyperthreading)----"
[[ "$BUILD_OPT" =~ "plt_samistb" ]] && ethtool -C eth0 rx-usecs 5000

# Binary config file
binary_cfg_file_fct

# Install and start
xbmc_fct

# Set STB id
echo "--------------------- STB ID management ------------------"
cd /NDS
source ./set_stb_id.sh $ETHINTER

# Capture  network traffic : tcpdump
if [ "$ISTCPDUMP" == "YES" ]
then
cd /NDS
echo "TCPDUMP launched..."
if [ "$PERFORMOUNT" == "YES" ] ; then ( ./ifup_net_cap.sh $ETHINTER $HOMEDIR &) ; else ( ./ifup_net_cap.sh $ETHINTER /NDS &) ; fi
fi
ln -s /NDS/fonts/ /NDS/lib/fonts
ln -s /NDS/lib/plugins/ /NDS/plugins

# NASC
[[ "$BUILD_OPT" =~ "enable_nasc"  ]] && nasc_fct

# Start SCD_Process
echo "--------------------- SCD_PROCESS Start -----------------"
echo "Change working directory to $BIN_DIR"
cd $BIN_DIR
echo "Launching application..."
export OPENSSL_ALLOW_PROXY_CERTS=1
#export QT_INSTALL_PATH=/NDS/lib
#export QT_PLUGIN_PATH=$QT_INSTALL_PATH/plugins/
#export QT_QPA_PLATFORM_PLUGIN_PATH=$QT_INSTALL_PATH/plugins/platforms
#export QT_QPA_FONTDIR=/NDS/fonts
#export QT_QPA_PLATFORM=fusion
export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:/lib/modules:/NDS/drivers:/lib/modules/cdi:/root/lib:/lib/modules:/NDS/lib
export EXTRA_LD_PRELOAD=/NDS/drivers/libResolveRefresh.so

# Diag Binary(NOTE:this function may modify the core_dump management)
diag_binary_fct

if [ "$NEEDMOUNTUSB" == "YES" ];then
  result=`mkdir -p /mnt/usb; mount -o sync /dev/${sd_usb}1 /mnt/usb 2>/dev/null`
  if [ $? != 0 ];then
    echo "No USB key detected"
    USB_PRESENCE="NO"
  fi
else
  USB_PRESENCE="NO"
fi

#Script to check and copy the files from ROOTFS to FLASH. only for Hawaii Platform
if [[ "$BUILD_OPT" =~ "WebkitQtHawaii" ]] && [ "$EXECFROM" != "NFS" ];then
    /NDS/flash_script.sh
fi

if [ "$USB_PRESENCE" == "YES" ] && [ "$EXEC_USB_SCRIPT" != "" ];then
    if [ -x "$EXEC_USB_SCRIPT" ];then
       echo "Executing USB Script in background..."
       ( $EXEC_USB_SCRIPT $run_count )&
    fi
fi

if [ "$USB_PRESENCE" == "YES" ] && [ "$CDIUSBLOG" == "YES" ];then
  mkdir -p $CDI_LOG_DIR
  export CDIT_FILE=$CDI_LOG_DIR/sf
  export EXTRA_LD_PRELOAD=/NDS/drivers/cdi_tracker.so:${EXTRA_LD_PRELOAD}
  export CDIT_PRINTF_IN_CONSOLE=1
  export WITH_CDI_TRACKER=1
fi

if [ "$KERNEL_LOG_FILE" != "" ];then
  result=`touch $KERNEL_LOG_FILE`
  if [ $? -eq 0 ];then
  echo "syslogd -S -O $KERNEL_LOG_FILE -s 0 "
  killall klogd.nds
  syslogd -S -O $KERNEL_LOG_FILE -s 0
  klogd
fi
fi

echo "CORE DUMP PATH :   `cat /proc/sys/kernel/core_pattern`"
echo "LOG_FILE       :   $LOG_FILE"
echo "KERNEL_LOG_FILE:   $KERNEL_LOG_FILE"
echo "DIAG LOG PATH  :   `cat /NDS/config/diag_binary.cfg | grep PATH=`"

gdl_mem_monitor

#Disabeling portal autostart for FUAPI purpose
[[ $METRO_DISABLED = "ON" ]] && sed -i 's/defaultAppId.>[[:digit:]]*</defaultAppId\">60</'  /NDS/resources/EPG_properties.xml

if [ "$NORUN" != "YES" ]
then
    RDATE="/usr/sbin/rdate"
    SETTIMEOFFSET="/NDS/bin/setTimeOffset"

    if [ -e ${RDATE} -a -x ${SETTIMEOFFSET} ]
    then
        echo "Create the file /tmp/time_offset (useful for WPE only in NO_PS and NO_PUMA option)"
        L_DATE=""
        L_INDEX=0
        SERVER=( "time.nist.gov"
                 "time-a.nist.gov"
                 "time-b.nist.gov"
                 "time-c.nist.gov"
                 "time-d.nist.gov"
               )
        while [ -z "$L_DATE" -a $L_INDEX -lt 5 ]
        do
            L_DATE=`${RDATE} -p ${SERVER[$L_INDEX]}`
            L_INDEX=$((L_INDEX +1))
        done
        [ -n "$L_DATE" ] && ${SETTIMEOFFSET} $L_DATE
    fi
    if [ "$ISGDBON" == "YES" ]
    then
       if [ -d /NDS/bin/gdb ]
       then
          echo "GDB already set up."
       else
          echo "Set up Processes for GDB."
          mkdir /NDS/bin/gdb
          for file in MW_Process APP_Process CA_Process
            do mv /NDS/bin/$file /NDS/bin/gdb/ ; echo "#!/bin/bash" > /NDS/bin/$file ; echo "echo $file $*" >> /NDS/bin/$file ; chmod 777 /NDS/bin/$file
          done
       fi

       echo "Starting GDB mode"
       check_gdbserver=`pidof gdbserver`
       [ $? == 0 ] && killall gdbserver 2>/dev/null

       if [ "$LOG_FILE" != "" ];then
         result=`touch $LOG_FILE`
         if [ $? -eq 0 ]; then
           touch_log_file="yes"
         fi
       fi
       if [ "$LOG_FILE" != "" ] && [ "$touch_log_file" = "yes" ]; then
         echo "trace log in $LOG_FILE"
         LD_PRELOAD=/NDS/drivers/libpoll.so:/lib/libdl.so:/NDS/drivers/libprio_range.so:libnds_eglext.so:${EXTRA_LD_PRELOAD} /usr/local/bin/gdbserver --multi localhost:$GDB_PORT  > $LOG_FILE  &
       else
         echo "trace log on terminal"
         LD_PRELOAD=/NDS/drivers/libpoll.so:/lib/libdl.so:/NDS/drivers/libprio_range.so:libnds_eglext.so:${EXTRA_LD_PRELOAD} /usr/local/bin/gdbserver --multi localhost:$GDB_PORT  &
       fi

    else
       echo "Starting normal mode"
       if [ -d /NDS/bin/gdb ]
       then
          echo "GDB set up - Revert back"
          for file in MW_Process APP_Process CA_Process
            do rm /NDS/bin/$file ;  mv /NDS/bin/gdb/$file /NDS/bin/$file
          done
          rm -rf /NDS/bin/gdb
       fi

       if [ "$LOG_FILE" != "" ];then
         result=`touch $LOG_FILE`
         if [ $? -eq 0 ]; then
           touch_log_file="yes"
         fi
       fi

       if [ "$LOG_FILE" != "" ] && [ "$touch_log_file" = "yes" ]; then
         echo "trace log in $LOG_FILE"
# Remove the '&' background as webbridge from Metrological started to use more than 80% (CSCva18601)
         LD_PRELOAD=/lib/libdl.so:/NDS/drivers/libprio_range.so:libnds_eglext.so:${EXTRA_LD_PRELOAD} /NDS/bin/MW_Starter_Process > $LOG_FILE 2>&1
       else
         echo "trace log on terminal"
         LD_PRELOAD=/lib/libdl.so:/NDS/drivers/libprio_range.so:libnds_eglext.so:${EXTRA_LD_PRELOAD} /NDS/bin/MW_Starter_Process
       fi
    fi
fi

