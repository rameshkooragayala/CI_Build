#!/bin/sh
###!/bin/bash

#
# This script generates a sysinit_component.cfg and spm.cfg files in line with the "sysinit_component" setting 
# in env. 
#
# *****
# To set a different STB_ID in sysinit_component.cfg and spm.cfg files    
# 1. The script can be run before start.sh 
#     eg. ./set_stb_id.sh 0A2ECCBF
# 2. Then, a new sysinit_component.cfg/spm.cfg files will be generated and 
#    copied to ~/config/ directory
#

function help(){
    echo ""
    echo "----------------------------------------"
    echo "-- Argument is missing : eth0 or br0	--"
    echo "-- 	So, Can't set the STB ID	--"
    echo "----------------------------------------"
    echo ""
}

# display the help
if [ "$1" == "" ]; then
    help
    exit 0
else
    ETHINTER="$1"
fi

param_setting=false

function stbid_from_mac()
{
	ID=0
        echo "select $ETHINTER interface"
	MAC=`/sbin/ifconfig | grep "$ETHINTER " | sed -e "s/.*HWaddr \(.*\)/\1/"`

	for DIGIT in `seq 0 17`
	do 
		#echo ${TTT:$DIGIT:1}
		CH=${MAC:$DIGIT:1}
		#echo $CH
		case $CH in
			[0-9] ) let ID="$ID * 16 + $CH";;
			[aA] ) let ID="$ID * 16 + 10";;
			[bB] ) let ID="$ID * 16 + 11";;
			[cC] ) let ID="$ID * 16 + 12";;
			[dD] ) let ID="$ID * 16 + 13";;
			[eE] ) let ID="$ID * 16 + 14";;
			[fF] ) let ID="$ID * 16 + 15";;
		esac
		#echo $ID
	done
	let ID="$ID % 4294967295"
}

# check param 
if [ "$2" = "" ]
then
    param_setting=true    
	stbid_from_mac
	STB_ID=$ID
else    
    STB_ID=`echo $2` 
    if test "$STB_ID" -ge 4294967295;
    then
    	echo "the CPE_ID shoudn't exceed the value 4294967295 !!!"
    	let STB_ID="$STB_ID % 4294967295"
    fi
    param_setting=true    
fi

export CPE_ID=$STB_ID

#echo "******* CPE_ID =" $STB_ID "**********"

#if [ "$param_setting" = "true" ]
#then
#    echo "Changing CPE_ID =" $STB_ID
#    cat config/sysinit_component.cfg | sed -e s/STB_ID=\".\*\"/STB_ID=\"$STB_ID\"/g > temp.cfg
#    mv -f temp.cfg config/sysinit_component.cfg
#    echo "New sysinit_component.cfg file is generated in config/" 

#    cat config/spm.cfg | sed -e s/STB_ID=.\*/STB_ID=$STB_ID/g > temp.cfg
#    mv -f temp.cfg config/spm.cfg
#    echo "New spm.cfg file is generated in config/" 

#    cat config/nagra_ca_kernel.cfg | sed -e s/STBID_STRING=\".\*\"/STBID_STRING=\"$STB_ID\"/g > temp.cfg
#    mv -f temp.cfg config/nagra_ca_kernel.cfg
#    cat config/nagra_ca_kernel.cfg | sed -e s/STBID_VALUE=.\*/STBID_VALUE=$STB_ID/g > temp.cfg
#    mv -f temp.cfg config/nagra_ca_kernel.cfg
#    echo "New nagra_ca_kernel.cfg file is generated in config/" 
#fi

export CPE_IPADDR_IPV4=`ifconfig $ETHINTER | grep "inet addr" | sed -e "s/.*inet addr:\(.*\)  Bcast.*/\1/"`
export CPE_IPADDR_LINK_IPV6=`ifconfig $ETHINTER | grep "inet6 addr" | grep "Scope:Link" | sed -e "s/.*inet6 addr:\(.*\) *Scope.*/\1/"`
export CPE_IPADDR_GLOBAL_IPV6=`ifconfig $ETHINTER | grep "inet6 addr" | grep "Scope:Global" | sed -e "s/.*inet6 addr:\(.*\) *Scope.*/\1/"`

if [ "$CPE_IPADDR_LINK_IPV6" == "" -a "$CPE_IPADDR_IPV4" != "" ]
then
    echo "******* CPE_IPADDR =" $CPE_IPADDR_IPV4 "**********"
elif [ "$CPE_IPADDR_LINK_IPV6" != "" -a "$CPE_IPADDR_IPV4" == "" ]
then
    echo "******* CPE_IPADDR_LINK_IPV6 =  " $CPE_IPADDR_LINK_IPV6   "**********"
    echo "******* CPE_IPADDR_GLOBAL_IPV6 =" $CPE_IPADDR_GLOBAL_IPV6 "**********"
else
    echo "******* CPE_IPADDR_IPV4 =       " $CPE_IPADDR_IPV4        "**********"
    echo "******* CPE_IPADDR_LINK_IPV6 =  " $CPE_IPADDR_LINK_IPV6   "**********"
    echo "******* CPE_IPADDR_GLOBAL_IPV6 =" $CPE_IPADDR_GLOBAL_IPV6 "**********"
fi

#sed -e '/"IP_address": "\(.*\)"/{s//"IP_address": "'$CPE_IPADDR'"/;:a' -e '$!N;$!ba' -e '}' config/ngi_wanipconnection.json > config/ngi_wanipconnection.tmp
#mv -f config/ngi_wanipconnection.tmp config/ngi_wanipconnection.json

