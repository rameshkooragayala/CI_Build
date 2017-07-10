#!/bin/sh -x

# Based on DMS's hgw_dms_startup.sh + CI/UPC-SAMGRVGW-03.run
STACKS="gw ipclient"
CHROOT_PATH="/chroots"

for STACK in $STACKS
do
	PATH=/usr/sbin:$PATH /NDS/create_fusion_chroot.sh $CHROOT_PATH $STACK /dev/shm/shared
	mount --bind /NDS/$STACK/NDS $CHROOT_PATH/$STACK/NDS
done

mount -t tmpfs tmpfs /mnt

# The call to firewall.sh breaks connectivity; ignore it
/usr/sbin/chroot /chroots/gw sed -i -e "/firewall.sh/s/^/# REMOVED FOR HGW /" /NDS/unified_startup.sh
/usr/sbin/chroot /chroots/ipclient sed -i -e "/firewall.sh/s/^/# REMOVED FOR HGW /" /NDS/unified_startup.sh

# We boot from an initramfs, but mount /NDS ourselves, and don't want it overridden, thank you very much.
/usr/sbin/chroot /chroots/gw sed -i -e "/mount \${HOMEDIR} \/NDS/s/^/# REMOVED FOR HGW /" /NDS/start.sh
/usr/sbin/chroot /chroots/ipclient sed -i -e "/mount \${HOMEDIR} \/NDS/s/^/# REMOVED FOR HGW /" /NDS/start.sh

if [[ -f /NDS/DiagBinary.sh ]] 
then
	Diag_binary_host=10.62.14.247;

	box_site=$(oneconfig.exe site|cut -d ":" -f 2|tr -d ' ')
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
		#default Setting
		*)
		echo "-------- DEFAULT (ISRAEL) CONFIGURATION (Site: $box_site) --------"
		Diag_binary_host=10.62.14.247;
		;;
	esac					

	source /NDS/DiagBinary.sh 1 ${Diag_binary_host}
	sleep 1 # Make sure there's a unique timestamp for this run
fi

export NORUN="YES" # TODO: find a better way to signal this
BASE_ARGS="$@"
GW_ARGS="$BASE_ARGS"
IPCLIENT_ARGS="`echo $BASE_ARGS | sed -e \"s/\<FORMAT\>//\"`"    
/usr/sbin/chroot /chroots/gw /NDS/start.sh $GW_ARGS
/usr/sbin/chroot /chroots/ipclient /NDS/start.sh $IPCLIENT_ARGS

# Remove unneeded devices
/usr/sbin/chroot /chroots/gw bash -c 'cd /dev/nds; for device in fpindicator fptextdisplay0 userinputfp fpsegmenteddisplay0 rf4ce avoutput* digitaloutput* audiomixer* viewport* surface* audiodecoder* videodecoder* blitter blender* pcmplayer* masterclock* clocksync* demux0/clockfilter* demux0/tschannel[6-7] demux0/tpidfilter{?,1[0-6]}; do rm -rf $device; done'
/usr/sbin/chroot /chroots/ipclient bash -c 'cd /dev/nds; for device in screader tuner* demux0/tschannel[0-5] demux0/rasp* demux0/remux* demux0/output* demux0/tpidfilter{1[7-9],[2-9]?,???}; do rm -rf $device; done'

# Create /tmp/shared, for components to share files (currently SYSTEMTIME only)
# Do not do this if it was already created via /NDS/create_fusion_chroot.sh
mkdir /tmp/shared
mount -t tmpfs tmpfs /tmp/shared
mount --bind /NDS/$STACK/NDS $CHROOT_PATH/$STACK/NDS
for STACK in $STACKS
do
	mkdir $CHROOT_PATH/$STACK/tmp/shared
	mount --bind /tmp/shared $CHROOT_PATH/$STACK/tmp/shared
done

current_dir=$(pwd)
cd /NDS/config
sed -i -e "s/INSTANCE_ID=.*/INSTANCE_ID=\"Main\"/" /NDS/config/diag.cfg
sed -i -e "s/DIAG_CLI_PORT=.*/DIAG_CLI_PORT=3212/" /NDS/config/diag.cfg
/chroots/gw/NDS/bin/FusionConfigImport CFG2BIN ./ *.[cC][fF][gG]
cd $current_dir

ulimit -c unlimited
echo "/host/core.%p" >> /proc/sys/kernel/core_pattern

cp /chroots/gw/root/run.sh /root/run.sh
/root/run.sh 
