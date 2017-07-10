#!/bin/sh

SCRIPT_NAME="boot_info.sh"

echo "${SCRIPT_NAME}: begin"

###########################################
# mount flash
###########################################

FLASH_DEV=""
BUILD_OPT=$(cat /NDS/options_start)
if [[ "${BUILD_OPT}" =~ "plt_samgw"  || "${BUILD_OPT}" =~ "plt_ciscogw" ]]; then
  FLASH_DEV="flash_symlnk0"
fi
if [[ "${BUILD_OPT}" =~ "plt_samistb" || "${BUILD_OPT}" =~ "plt_samipc" ]]; then
  FLASH_DEV="mmcblk0p12"
fi

if [ "${FLASH_DEV}" == "" ]; then
  echo "${SCRIPT_NAME}: flash device not defined!"
  exit -1
fi

MOUNT_POINT="/mnt/flash"

mkdir -p ${MOUNT_POINT}

mount -t ext3 /dev/${FLASH_DEV} ${MOUNT_POINT} &> /dev/null
flash_mount=`mount | grep /dev/${FLASH_DEV} | grep ${MOUNT_POINT} &> /dev/null`
if [ $? != 0 ] ;then
  echo "${SCRIPT_NAME}: flash not mounted!"
  rmdir ${MOUNT_POINT}
  exit -2
fi

###########################################
# WakeUp Cause
###########################################

if [ ! -x /NDS/bin/xpower_getwakeup ]; then
  echo "${SCRIPT_NAME}: xpower_getwakeup not found!"
  umount ${MOUNT_POINT}
  rmdir ${MOUNT_POINT}
  exit -3
fi

WAKEUP_CAUSE=`/NDS/bin/xpower_getwakeup | grep "$@@$Wakeup" | cut -d ":" -f 2`

echo "${SCRIPT_NAME}: Wakeup cause: ${WAKEUP_CAUSE}"

# FUSIONOS/FOS_INC/nds/xpower.h
echo -n "${SCRIPT_NAME}: Wakeup cause: "
case "${WAKEUP_CAUSE}" in
  0x0001)
    echo "POWER_ON"
    ;;
  0x0002)
    echo "REMOTE"
    ;;
  0x0004)
    echo "FRONTPANEL"
    ;;
  0x0004)
    echo "TIMER"
    ;;
  0x0010)
    echo "WATCHDOG..."
    ;;
  0x0020)
    echo "AUTH_FAILURE"
    ;;
  0x0040)
    echo "CLIENT_RESET"
    ;;
  0x0080)
    echo "WAKE_ON_LAN"
    ;;
  0x0100)
    echo "WAKE_ON_MOCA"
    ;;
  0x0200)
    echo "DRIVER_INTERNAL..."
    ;;
  * )
    echo "Unknown"
    ;;
esac
###########################################

###########################################
# Shutdown Reason
###########################################

SHUTDOWN_REASON=`cat ${MOUNT_POINT}/shutdown_reason | cut -d ";" -f 1`
	
echo "${SCRIPT_NAME}: Shutdown reason: ${SHUTDOWN_REASON}"

if [ ${WAKEUP_CAUSE} == 0x0200 ]; then
  SHUTDOWN_REASON=17
fi
if [ ${WAKEUP_CAUSE} == 0x0010 ]; then
  SHUTDOWN_REASON=18
fi

# CMS_SYSTEM_INFRASTRUCTURE/SYSTEMCOMMON/inc/system_shutdown_reasons.h
echo -n "${SCRIPT_NAME}: Shutdown reason: "
case "${SHUTDOWN_REASON}" in
   0)
    echo "undefined"
    ;;
   1)
    echo "unexpected"
    ;;
   2)
    echo "software update"
    ;;
   3)
    echo "maintenance phase"
    ;;
   4)
    echo "passive standby"
    ;;
   5)
    echo "factory reset by user"
    ;;
   6)
    echo "factory reset by user while retaining recordings"
    ;;
   7)
    echo "disk error"
    ;;
   8)
    echo "power off by Panorama server"
    ;;
   9)
    echo "factory reset by Panorama server"
    ;;
  10)
    echo "disk format by Panorama server"
    ;;
  11)
    echo "disk format by user"
    ;;
  12)
    echo "STM process crash"
    ;;
  13)
    echo "SCD process crash"
    ;;
  14)
    echo "MWS process crash"
    ;;
  15)
    echo "CAK process crash"
    ;;
  16)
    echo "APP process crash"
    ;;
  17)
    echo "kernel crash..."
    ;;
  18)
    echo "PWM process crash..."
    ;;
  19)
    echo "last"
    ;;
  * )
    echo "Unknown"
    ;;
esac

###########################################
# unmount flash
###########################################

umount ${MOUNT_POINT}
rmdir ${MOUNT_POINT}

###########################################

echo "${SCRIPT_NAME}: end"
