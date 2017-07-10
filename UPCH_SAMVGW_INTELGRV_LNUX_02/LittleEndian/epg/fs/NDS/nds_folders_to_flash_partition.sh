#!/bin/sh

SCRIPT_NAME=${0##*/}

echo "${SCRIPT_NAME}: begin"

fallback_to_ram()
{
  echo "${SCRIPT_NAME}: $1"

  ML_DIR=/NDS/metrological
  echo "${SCRIPT_NAME}: remove ${ML_DIR} directory"
  rm -rf ${ML_DIR}

  SYSINIT_CFG=/NDS/config/sysinit.cfg
  echo "${SCRIPT_NAME}: patch ${SYSINIT_CFG} file"

  echo -n "" > ${SYSINIT_CFG}.mod
  DONE=false
  RL20=false
  RL2=false

  until ${DONE}
  do
    read line || DONE=true
    case "${line}" in
      NUM_CHILD_PROCESSES*)
        number=`echo ${line} | cut -d"=" -f 2`
        echo "NUM_CHILD_PROCESSES=$((number-1))" >> ${SYSINIT_CFG}.mod
        line=""
      ;;

      *RING_LEVEL_20])
        RL20=true
      ;;

      *RING_LEVEL_2])
        RL2=true
      ;;

      NUM_COMPONENTS*)
        if [ "${RL20}" == "true" ]; then
          RL20=false
          echo "NUM_COMPONENTS=0" >> ${SYSINIT_CFG}.mod
          line=""
        elif [ "${RL2}" == "true" ]; then
          RL2=false
          number=`echo ${line} | cut -d"=" -f 2`
          if [[ $((number)) -ge 9 ]]; then # only useful for STB Profiler builds: do not launch STBP_LocalAgent_WPP
            echo "NUM_COMPONENTS=$((number-1))" >> ${SYSINIT_CFG}.mod
            line=""
          fi
        fi
      ;;
    esac

    if [ "${line}" != "" ]; then
      echo "${line}" >> ${SYSINIT_CFG}.mod
    fi

  done < ${SYSINIT_CFG}

  mv ${SYSINIT_CFG}.mod ${SYSINIT_CFG}

  VERSION_CFG=/NDS/config/version.cfg
  echo "${SCRIPT_NAME}: patch ${VERSION_CFG} file"
  cat ${VERSION_CFG} | sed -e "s#\(.*\)\"\(.*\)\"#\1\"RAM_\2\"#g" > ${VERSION_CFG}.mod
  mv ${VERSION_CFG}.mod ${VERSION_CFG}

  exit -1
}


DEBUG=1

HARD_MODEL=$(cat /dev/nds/versioninformation | sed -n s/^MODEL_NAME://p | cut -c1-9)
BROWSER_KIND=$(cat /NDS/options_start | sed -n s/^Webkit//p | cut -c1-3)
if [[ ( ${HARD_MODEL} != "SMT-C5400"  &&  ${HARD_MODEL} != "SMT-E6400" ) || ( ${BROWSER_KIND} != "WPE" ) ]]; then
#                         iSTB                              IPC                                   WPE
  echo "${SCRIPT_NAME}: No need to do anything"

else

  DEV_NAME="mmcblk0p12"
  OLD_DIR="/NDS"
  MOUNT_POINT="/flash"
  NEW_DIR="${MOUNT_POINT}${OLD_DIR}"

  DEV_TYPE=`fdisk -l | grep ${DEV_NAME} | awk '{print $6}'`

  if [ "Linux" != "${DEV_TYPE}" ]; then

    echo "${SCRIPT_NAME}: partition is not a Linux partition (${DEV_TYPE})"
    echo "${SCRIPT_NAME}: ${OLD_DIR} folder will stay in initramfs"

  else
    echo "${SCRIPT_NAME}: Linux partition detected"

    echo "${SCRIPT_NAME}: creating mount point ${MOUNT_POINT}"
    mkdir -p ${MOUNT_POINT}

    echo "${SCRIPT_NAME}: mounting partition"
    mount -t ext3 /dev/${DEV_NAME} ${MOUNT_POINT}
    flash_mount=`mount | grep /dev/${DEV_NAME} | grep ${MOUNT_POINT} &> /dev/null`
    if [ $? != 0 ] ;then
      rmdir ${MOUNT_POINT}
      fallback_to_ram  "could not mount flash: keep in RAM"
    fi

    PDM_FILE_NAME=99
    TRIGGER_FILE=format_flash
    /NDS/bin/disk_partition ${DEV_NAME} ${MOUNT_POINT} ${PDM_FILE_NAME} ${TRIGGER_FILE}

    if [[(-f ${MOUNT_POINT}/${TRIGGER_FILE}) || ($1 == 'FORMAT')]]; then

      echo "${SCRIPT_NAME}: remove trigger file"
      rm -f ${MOUNT_POINT}/${TRIGGER_FILE}

      echo "${SCRIPT_NAME}: save patched PDM file"
      cp ${MOUNT_POINT}/${PDM_FILE_NAME} /tmp/

      echo "${SCRIPT_NAME}: unmounting partition"
      umount ${MOUNT_POINT}

      echo "${SCRIPT_NAME}: formatting flash..."
      COUNT=0
      while [ ${COUNT} -ne 3 ];
      do
        # same action as in factory_reset.sh/storage_helper
        /sbin/mkfs.ext3 -b 2048 /dev/${DEV_NAME}
        if [ $? != 0 ] ;then
	  COUNT=$((COUNT+1))
          if [ ${COUNT} -eq 2 ];then
            fallback_to_ram  "could not format flash: keep in RAM"
          fi
        else
          COUNT=3
        fi
      done

      # NOT same action as in factory_reset.sh/storage_helper: do not disable linux check
      # tune2fs -c0 /dev/${DEV_NAME}

      echo "${SCRIPT_NAME}: mounting partition"
      mount /dev/${DEV_NAME} ${MOUNT_POINT}
      flash_mount=`mount | grep /dev/${DEV_NAME} | grep ${MOUNT_POINT} &> /dev/null`
      if [ $? != 0 ] ;then
        rmdir ${MOUNT_POINT}
        fallback_to_ram  "could not mount flash: keep in RAM"
      fi

      echo "${SCRIPT_NAME}: restore patched PDM file"
      cp /tmp/${PDM_FILE_NAME} ${MOUNT_POINT}/

      # maybe sync?
    fi

    echo "${SCRIPT_NAME}: move NDS sub-folders to flash partition"
:
    echo "${SCRIPT_NAME}: memory information before (with drop_caches)"
    echo 2 > /proc/sys/vm/drop_caches
    cat /proc/meminfo | egrep -e 'Mem'

    if [ "${DEBUG}" == "1" ]; then

      echo "${SCRIPT_NAME}-DEBUG: BEFORE: Size of ${OLD_DIR}"
      du -s -k ${OLD_DIR}

    fi

    if [ -d ${NEW_DIR} ]; then
      if [ "${DEBUG}" == "1" ]; then

        echo "${SCRIPT_NAME}-DEBUG: BEFORE: Size of ${NEW_DIR}"
        du -s -k ${NEW_DIR}

      fi

      echo "${SCRIPT_NAME}: removing ${NEW_DIR} from previous boot"
      time rm -rf ${NEW_DIR}
    fi

    echo "${SCRIPT_NAME}: creating destination directory ${NEW_DIR}"
    mkdir -p ${NEW_DIR}

    echo "${SCRIPT_NAME}: moving to ${OLD_DIR}"
    cd ${OLD_DIR}

    # loop through all dirs in ${OLD_DIR}
    error=0
    time ls -1 -d */ | sed 's/\///g' | while read THIS_DIR
    do
      cp -fra ${THIS_DIR} ${NEW_DIR}
      if [ $? -ne 0 ]; then
        echo "${SCRIPT_NAME}: FAILED to copy ${THIS_DIR} from RAM to flash"
        error=$(expr ${error} + 1)
      else
        if [ "${DEBUG}" == "1" ]; then
          echo "${SCRIPT_NAME}-DEBUG: copied ${THIS_DIR} from RAM to flash"
       fi
      fi
    done

    #echo "${SCRIPT_NAME}-TST-NAIM force error to test fallback"
    #error=$(expr ${error} + 1)

    if [ ${error} -ne 0 ]; then

      fallback_to_ram  "could not copy all directories to flash: keep in RAM"

    else

      time ls -1 -d */ | sed 's/\///g' | while read THIS_DIR
      do

        error=0

        rm -rf ${THIS_DIR}
        if [ $? -ne 0 ]; then
          echo "${SCRIPT_NAME}: FAILED to remove ${THIS_DIR} from RAM"
          error=$(expr ${error} + 1)
        fi

        ln -s ${NEW_DIR}/${THIS_DIR} ${THIS_DIR}
        if [ $? -ne 0 ]; then
          echo "${SCRIPT_NAME}: FAILED to create symlink for ${THIS_DIR} to flash"
          error=$(expr ${error} + 1)
        fi

        if [ ${error} -eq 0 ]; then
          if [ "${DEBUG}" == "1" ]; then
            echo "${SCRIPT_NAME}-DEBUG: moved ${THIS_DIR} from RAM to flash and linked"
          fi
        fi

      done

      # We do not sync after copy (gain 9s)
      # echo "${SCRIPT_NAME}: sync flash"
      # time sync -f /dev/${DEV_NAME}

    fi

    echo "${SCRIPT_NAME}: memory information after (with drop_caches)"
    echo 2 > /proc/sys/vm/drop_caches
    cat /proc/meminfo | egrep -e 'Mem'

    if [ "${DEBUG}" == "1" ]; then

      echo "${SCRIPT_NAME}-DEBUG: AFTER: Size of ${OLD_DIR}"
      du -s -k ${OLD_DIR}

      echo "${SCRIPT_NAME}-DEBUG: AFTER: Size of ${NEW_DIR}"
      du -s -k ${NEW_DIR}

    fi

  fi

fi

echo "${SCRIPT_NAME}: end"
