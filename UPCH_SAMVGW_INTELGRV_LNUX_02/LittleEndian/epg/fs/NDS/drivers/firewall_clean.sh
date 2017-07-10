#!/bin/sh

# reset default policies
iptables -t filter -P INPUT ACCEPT
iptables -t filter -P OUTPUT ACCEPT
iptables -t filter -P FORWARD ACCEPT

# clear filter 
iptables -F
iptables -X


if [ -d /proc/sys/net/ipv6 ] ; then
  # reset default policies
  ip6tables -t filter -P INPUT ACCEPT
  ip6tables -t filter -P OUTPUT ACCEPT
  ip6tables -t filter -P FORWARD ACCEPT

  # clear filter 
  ip6tables -F
  ip6tables -X
fi
