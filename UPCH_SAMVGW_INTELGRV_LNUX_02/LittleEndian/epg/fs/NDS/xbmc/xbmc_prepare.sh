#!/bin/sh


#CQ1006723 /dev/loop workaround
if ! [ -e /dev/loop0 ]
then
	mknod /dev/loop0 b 7 0
fi




#NASC prepare

	xbmc_fs_path=$1"/xbmc_jail_fs"
	#size=$2
	#dd if=/dev/zero of=$xbmc_fs_path bs=4k count=262144
	#dd if=/dev/zero of=$xbmc_fs_path bs=1k count=$size

cat <<END |  /sbin/mkfs.ext3 -q $xbmc_fs_path
y
END



	xbmc_jail_path="/tmp/xbmc_jail/"
	mount -o loop,data=ordered $xbmc_fs_path $xbmc_jail_path 

#jail filling
	cd $xbmc_jail_path
	mkdir bin usr xbmc proc sys tmp etc lib usr/lib 

	mount -o bind /NDS/xbmc/ $xbmc_jail_path/xbmc/
        cp -r /NDS/xbmc/home_xbmc/ $xbmc_jail_path/.xbmc
        chown -Rh NDS_U10:10000 $xbmc_jail_path/xbmc
        chown -Rh NDS_U10:10000 $xbmc_jail_path/.xbmc
	chmod -R u+rwx $xbmc_jail_path/.xbmc
	chmod -R u+rwx $xbmc_jail_path/xbmc/
        chmod -R a+rx $xbmc_jail_path/.xbmc
        chmod -R a+rx $xbmc_jail_path/xbmc/
        #cp -r /lib/ $xbmc_jail_path/
        #cp -r /usr/lib/ $xbmc_jail_path/usr/
	mount -o bind /lib/ $xbmc_jail_path/lib/
	mount -o bind /usr/lib/ $xbmc_jail_path/usr/lib/
	mount -o bind /etc/ $xbmc_jail_path/etc
	mount -o bind /tmp/ $xbmc_jail_path/tmp/
	mount -o bind /proc $xbmc_jail_path/proc
	mount -o bind /sys $xbmc_jail_path/sys

        #cp /NDS/xbmc/XBMC_init $xbmc_jail_path/xbmc/
        #cp -r /NDS/xbmc/env_xbmc/ $xbmc_jail_path/xbmc/
        #cp -r /NDS/xbmc/lib/ $xbmc_jail_path/xbmc/

#	cp -r /etc $xbmc_jail_path/
#	cp -r /temp $xbmc_jail_path/
#	cp -r /tmp/time_offset $xbmc_jail_path/tmp/
#	cp -r /tmp/resolv.conf $xbmc_jail_path/tmp/


	cd /



