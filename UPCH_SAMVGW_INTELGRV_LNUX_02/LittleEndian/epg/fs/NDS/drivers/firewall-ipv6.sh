#!/bin/sh

if [ ! -d /proc/sys/net/ipv6 ] ; then
  echo "No IPv6 support !"
  exit 0
fi

cat /proc/cmdline |grep nfsroot &>/dev/null
if [ $? == 0 ] ; then
  if_in=eth0
else
  if_in=br0
fi

# clear filter 
ip6tables -F
ip6tables -X

# chain for allowing servers
ip6tables -t filter -N allow_in
ip6tables -t filter -N allow_in_debug

ip6tables -t filter -i ${if_in} -A INPUT -j allow_in_debug
ip6tables -t filter -i ${if_in} -A INPUT -j allow_in

# allow everything on localhost
ip6tables -t filter -i lo -A INPUT -j ACCEPT
# allow related, established
ip6tables -t filter -i ${if_in} -A INPUT -p tcp -m state --state RELATED,ESTABLISHED -j  ACCEPT
ip6tables -t filter -i ${if_in} -A INPUT -p udp -m state --state RELATED,ESTABLISHED -j  ACCEPT

# log tcp connections we don't know about, for debug purposes only 
nds_firewall=$(oneconfig.exe nds_firewall|cut -f 2 -d ':'|tr -d ' ')
if [ x"$nds_firewall" == "xdebug" ] ; then
  ip6tables -t filter -i ${if_in} -A INPUT -p tcp --syn -j LOG --log-prefix "tcp inbound: " # -m limit --limit 1/s --limit-burst 1 # limit module is not yet enabled
  ip6tables -t filter -i ${if_in} -A INPUT -p udp -j LOG --log-prefix "udp inbound: "
fi

# telnet, debug only
ip6tables -t filter -i ${if_in} -A allow_in_debug -p tcp --dport 23 -j ACCEPT
# diagcli, debug only
ip6tables -t filter -i ${if_in} -A allow_in_debug -p tcp --dport 3210 -j ACCEPT
# rprof
ip6tables -t filter -i ${if_in} -A allow_in_debug -p udp --dport 16675 -j ACCEPT
ip6tables -t filter -i ${if_in} -A allow_in_debug -p tcp --dport 16676 -j ACCEPT
# webkit inspector
ip6tables -t filter -i ${if_in} -A allow_in_debug -p tcp --dport 9999 -j ACCEPT


# NDS
# second screen/remote 
ip6tables -t filter -i ${if_in} -A allow_in -p tcp --dport 5900 -j ACCEPT
# streaming removed on 24 june 2016
# RNC
# keep both ports for migration
ip6tables -t filter -i ${if_in} -A allow_in -p udp --dport 49501 -j ACCEPT
ip6tables -t filter -i ${if_in} -A allow_in -p udp --dport 29501 -j ACCEPT
# PAN
ip6tables -t filter -i ${if_in} -A allow_in -p tcp --dport 7547 -j ACCEPT
# icmpv6 
ip6tables -t filter -i ${if_in} -A allow_in -p icmpv6 -j ACCEPT
# NTP server
ip6tables -t filter -i ${if_in} -A allow_in -p udp --dport 123 -j ACCEPT
# Box 2 Box communication
ip6tables -t filter -i ${if_in} -A allow_in -p tcp --dport 1234 -j ACCEPT
ip6tables -t filter -i ${if_in} -A allow_in -p tcp --dport 1235 -j ACCEPT

# DIAL
ip6tables -t filter -i ${if_in} -A allow_in -p tcp --dport 8080 -j ACCEPT
ip6tables -t filter -i ${if_in} -A allow_in -p tcp --dport 9080 -j ACCEPT


# Samsung
# Samsung NGI
ip6tables -t filter -i ${if_in} -A allow_in -p udp --dport 5007 -j ACCEPT

# default policies
ip6tables -t filter -P INPUT DROP
ip6tables -t filter -P OUTPUT ACCEPT
ip6tables -t filter -P FORWARD ACCEPT # this is needed for the packets to go through the bridge
