#!/bin/sh -x

CHROOT_PATH="$1"
STACK="$2"
SHARED_IPC_PATH="$3"

# TODO: add error checking

STACK_ROOT=$CHROOT_PATH/$STACK

# Create the chroot main directory
mkdir -p $STACK_ROOT

# Shared paths
for DIR in / /proc /dev /proc/bus/usb /dev/pts /sys /vfs
do
        mount --rbind "$DIR" "$STACK_ROOT/$DIR"
done

# Unique tmpsfs paths
for DIR in /root /NDS /tmp /dev/shm /fifo /dev/nds
do
        mount -t tmpfs tmpfs "$STACK_ROOT/$DIR"
done

# /host within the chroot
mkdir -p $STACK_ROOT/host
mount --rbind /host/chroot_jails/$STACK $STACK_ROOT/host

# For UPC, this works on NFS builds and builds that have oneconfig nds_no_removal set. If it's not, the file cdi_mknods is deleted in /etc/rc3.d/S100NDS, so we fall back to copying /dev/nds/
chroot $STACK_ROOT cdi_mknods || cp -a /dev/nds $STACK_ROOT/dev

# Shared XIPC directory.
if ! grep "tmpfs $SHARED_IPC_PATH" /proc/mounts > /dev/null 2>&1
then
        mkdir -p $SHARED_IPC_PATH
        mount -t tmpfs tmpfs $SHARED_IPC_PATH
fi
mkdir -p $STACK_ROOT/$SHARED_IPC_PATH
mount --bind $SHARED_IPC_PATH $STACK_ROOT/$SHARED_IPC_PATH
