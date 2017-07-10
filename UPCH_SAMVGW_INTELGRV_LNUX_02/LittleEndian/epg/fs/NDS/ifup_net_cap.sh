#!/bin/bash

# This script perfroms tcpdump on a sepcific interface Arg_1 and the capture logs are redirected in a file in the folder Arg_2

function help(){
    echo ""
    echo "----------------------------------------"
    echo "-- Arguments are missing !!!!         --"
    echo "-- Arg_1 : interface name	==> br0 | eth0 | ce00 | eth1  --"
    echo "-- Arg_2 : directory for the 'capture' file	==> /tmp | /NDS/ | /host/  --"	
    echo "-- Example : ./ifup_net_cap.sh  br0  /tmp/  --"	
    echo "----------------------------------------"
    echo ""
}

# Display the help section
if [ "$1" == "" -o "$2" == "" ]; then
    help
    exit 0
else
    IFACE="$1"
	FOLDER="$2"
fi

s_i=0
#ifconfig="/sbin/ifconfig"
IFCONFIG=`which ifconfig`
FILTER=""
FILE_PREFIX="capture_tcpdump_on"
#tcpdump="/usr/bin/tcpdump"
TCPDUMP=`which tcpdump`
#PS_ARGS="ax"
PS_ARGS="w"

if_down()
{
    IS_UP=`${IFCONFIG} ${IFACE} | grep BROADCAST | cut -d ' ' -f 11`
    while [ "$IS_UP" != "UP" ]
    do
        usleep 100000;
        IS_UP=`${IFCONFIG} ${IFACE} | grep BROADCAST | cut -d ' ' -f 11`
    done
}

if_up()
{
    IS_TCP="`ps ${PS_ARGS}|grep tcpdump | grep -v grep`"
    while [ -n "$IS_TCP" ]
    do
        usleep 100000;
        IS_TCP=`ps ${PS_ARGS}|grep tcpdump | grep -v grep`
    done
}


IS_UP=`${IFCONFIG} ${IFACE} | grep BROADCAST | cut -d ' ' -f 11`
if [ "$IF_UP" != "UP" ]
then
    if_down;
fi
while [ 1 ]
do
    `${TCPDUMP} -ns 0 -i ${IFACE} -w ${FOLDER}/${FILE_PREFIX}_${IFACE}_${s_i}.pcap ${FILTER}` &
    s_i=$((s_i+1))
    if_up;
    if_down;
done

