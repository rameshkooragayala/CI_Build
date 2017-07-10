#!/bin/sh

if [ "$#" == 0 -o  "$#" == 1 ] ; then
  echo "No extra ports to open"
  exit 0
fi

cat /proc/cmdline |grep nfsroot &>/dev/null
if [ $? == 0 ] ; then
  if_in=eth0
else
  if_in=br0
fi


open_port()
{
    echo "Opening $proto:$port on $if_in"
    iptables -t filter -i ${if_in} -A allow_in -p $proto --dport $port -j ACCEPT
    ip6tables -t filter -i ${if_in} -A allow_in -p $proto --dport $port -j ACCEPT
}

port=""
proto=""

while [ $# -gt 0 ]
do
  if [ -z $proto ] ; then
      proto=$1
  else
      port=$1
      open_port
      proto=""
      port=""
  fi
  shift
done