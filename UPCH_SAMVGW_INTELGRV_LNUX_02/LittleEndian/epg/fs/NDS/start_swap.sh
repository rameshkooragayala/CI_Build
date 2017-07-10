#!/bin/sh

SCRIPT_NAME="start_swap.sh"

SIZE_MB=$1
if [ -z ${SIZE_MB} ];
then
  echo "${SCRIPT_NAME}: need a size in MiB"
  exit -1
fi

BLOCK_SIZE=512
NUMBER_OF_BLOCKS=$(expr ${SIZE_MB} \* 1024 \* 2)
echo "${SCRIPT_NAME}: will use a file size of ${NUMBER_OF_BLOCKS} blocks of ${BLOCK_SIZE} bytes"

SLEEP_TIME=30
PDM_FILE=/tmp/diag
COUNT=${SLEEP_TIME}
SWAP_PATH=/mnt/nds/dev_4/part_1
SWAP_FILE_RIGHTS=600

echo "${SCRIPT_NAME}: let's sleep for ${SLEEP_TIME} seconds"
sleep ${SLEEP_TIME}

while [ ! -f ${PDM_FILE} ]; do
#  echo "${SCRIPT_NAME}: let's sleep for 1 more second"
  sleep 1
  COUNT=$(expr ${COUNT} + 1)
done

echo "${SCRIPT_NAME}: have waited ${COUNT} seconds for HDD to be mounted"

SWAP_PATH=`cat ${PDM_FILE}`;

if [ ! -d ${SWAP_PATH} ]; then
  echo "${SCRIPT_NAME}: ${SWAP_PATH} does not exist - exit"
  exit -1
else
  echo "${SCRIPT_NAME}: HDD mounted on ${SWAP_PATH}"
fi

SWAP_FILE=${SWAP_PATH}/swapfile
SWAP_FILE_TYPE=${SWAP_PATH}/swaptype
EXPECTED_TYPE=clear
SWAP_AREA=${SWAP_FILE}

echo "${SCRIPT_NAME}: dump before"
cat /proc/meminfo | grep Swap

if [ -f  ${SWAP_FILE} ] ; then
  SWAP_TYPE=`cat ${SWAP_FILE_TYPE}`
  if [ "${SWAP_TYPE}" != "${EXPECTED_TYPE}" ]; then
    echo "${SCRIPT_NAME}: switch to '${EXPECTED_TYPE}' ('${SWAP_TYPE}')"
    rm -fr ${SWAP_FILE}
  fi
fi

if [ ! -f  ${SWAP_FILE} ] ; then
  echo "${SCRIPT_NAME}: create ${SWAP_FILE} with size of ${NUMBER_OF_BLOCKS} blocks of ${BLOCK_SIZE} bytes"
  dd if=/dev/zero of=${SWAP_FILE} bs=${BLOCK_SIZE} count=${NUMBER_OF_BLOCKS}

  echo "${SCRIPT_NAME}: ${SWAP_FILE} attributes after creation"
  ls -lh ${SWAP_FILE}

  echo "${SCRIPT_NAME}: change ${SWAP_FILE} rights to ${SWAP_FILE_RIGHTS}"
  chmod ${SWAP_FILE_RIGHTS} ${SWAP_FILE}

  if [ -x /NDS/config/mkswap ]
  then
    echo "${SCRIPT_NAME}: delete NDS mkswap"
    rm -f /NDS/config/mkswap
  fi

  echo "${SCRIPT_NAME}: create swap area as ${SWAP_AREA}"
  mkswap ${SWAP_AREA}

  echo "${SCRIPT_NAME}: set swap type to '${EXPECTED_TYPE}'"
  echo "${EXPECTED_TYPE}" > ${SWAP_FILE_TYPE}

  echo "${SCRIPT_NAME}: swap area created as ${SWAP_AREA}"
else
  echo "${SCRIPT_NAME}: swap area already exists as ${SWAP_AREA}"
fi

echo "${SCRIPT_NAME}: enable swap area as ${SWAP_AREA}"
swapon ${SWAP_AREA}

echo "${SCRIPT_NAME}: ${SWAP_FILE} attributes after enabling"
ls -lh ${SWAP_FILE}

echo "${SCRIPT_NAME}: dump after"
cat /proc/meminfo | grep Swap
