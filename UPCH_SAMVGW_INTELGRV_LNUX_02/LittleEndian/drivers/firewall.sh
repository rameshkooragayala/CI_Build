#!/bin/sh

cat /proc/cmdline |grep nfsroot &>/dev/null
if [ $? == 0 ] ; then
  if_in=eth0
else
  if_in=br0
fi

# clear filter 
iptables -F
iptables -X

# chain for allowing servers
iptables -t filter -N allow_in
iptables -t filter -N allow_in_debug

iptables -t filter -i ${if_in} -A INPUT -j allow_in_debug
iptables -t filter -i ${if_in} -A INPUT -j allow_in

# allow everything on localhost
iptables -t filter -i lo -A INPUT -j ACCEPT
# allow related, established
iptables -t filter -i ${if_in} -A INPUT -p tcp -m state --state RELATED,ESTABLISHED -j  ACCEPT
iptables -t filter -i ${if_in} -A INPUT -p udp -m state --state RELATED,ESTABLISHED -j  ACCEPT

# Samsung, protocol to/from the CM
iptables -t filter -i eth0.4093 -A INPUT -d 172.24.167.188 -s 172.24.167.187  -j ACCEPT

# log tcp connections we don't know about, for debug purposes only 
nds_firewall=$(oneconfig.exe nds_firewall|cut -f 2 -d ':'|tr -d ' ')
if [ x"$nds_firewall" == "xdebug" ] ; then
  iptables -t filter -i ${if_in} -A INPUT -p tcp --syn -j LOG --log-prefix "tcp inbound: " # -m limit --limit 1/s --limit-burst 1 # limit module is not yet enabled
  iptables -t filter -i ${if_in} -A INPUT -p udp -j LOG --log-prefix "udp inbound: "
fi

# telnet, debug only
iptables -t filter -i ${if_in} -A allow_in_debug -p tcp --dport 23 -j ACCEPT
# diagcli, debug only
iptables -t filter -i ${if_in} -A allow_in_debug -p tcp --dport 3210 -j ACCEPT
# rprof
iptables -t filter -i ${if_in} -A allow_in_debug -p udp --dport 16675 -j ACCEPT
iptables -t filter -i ${if_in} -A allow_in_debug -p tcp --dport 16676 -j ACCEPT
# webkit inspector
iptables -t filter -i ${if_in} -A allow_in_debug -p tcp --dport 9999 -j ACCEPT



# NDS
# upnp / http
iptables -t filter -i ${if_in} -A allow_in -p tcp --dport 29153 -j ACCEPT
# upnp / ssdp
iptables -t filter -i ${if_in} -A allow_in -p udp --dport 1900 -j ACCEPT
iptables -t filter -i ${if_in} -A allow_in -p udp --dport 29154 -j ACCEPT
# second screen/remote 
iptables -t filter -i ${if_in} -A allow_in -p tcp --dport 5900 -j ACCEPT
# xbmc
iptables -t filter -i ${if_in} -A allow_in -p tcp --dport 62137 -j ACCEPT
# xbmc, gena events
iptables -t filter -i ${if_in} -A allow_in -p tcp --dport 62138 -j ACCEPT
iptables -t filter -i ${if_in} -A allow_in -p udp --dport 62138 -j ACCEPT
# RNC
iptables -t filter -i ${if_in} -A allow_in -p udp --dport 29501 -j ACCEPT
# PAN
iptables -t filter -i ${if_in} -A allow_in -p tcp --dport 7547 -j ACCEPT
# IGMP for multicast UPNP
iptables -t filter -i ${if_in} -A allow_in -p igmp -j ACCEPT
# NTP server
iptables -t filter -i ${if_in} -A allow_in -p udp --dport 123 -j ACCEPT
# Box 2 Box communication
iptables -t filter -i ${if_in} -A allow_in -p tcp --dport 1234 -j ACCEPT
iptables -t filter -i ${if_in} -A allow_in -p tcp --dport 1235 -j ACCEPT
# hawaii / websocket debug
iptables -t filter -i ${if_in} -A allow_in -p tcp --dport 4480 -j ACCEPT
iptables -t filter -i ${if_in} -A allow_in -p tcp --dport 4481 -j ACCEPT
iptables -t filter -i ${if_in} -A allow_in -p tcp --dport 4443 -j ACCEPT
iptables -t filter -i ${if_in} -A allow_in -p tcp --dport 4444 -j ACCEPT
iptables -t filter -i ${if_in} -A allow_in -p tcp --dport 9517 -j ACCEPT


# DIAL
iptables -t filter -i ${if_in} -A allow_in -p tcp --dport 8080 -j ACCEPT
iptables -t filter -i ${if_in} -A allow_in -p tcp --dport 9080 -j ACCEPT

# allow range for UDP as a workaround (XBMC)
iptables -t filter -i ${if_in} -A allow_in -p udp --dport 1025:65535 -j ACCEPT

# default policies
iptables -t filter -P INPUT DROP
iptables -t filter -P OUTPUT ACCEPT
iptables -t filter -P FORWARD ACCEPT # this is needed for the packets to go through the bridge
