#!/bin/sh

build=$1

rm -r /host/$build/fs/NDS/results
mkdir /host/$build/fs/NDS/results
cp /NDS/*.xml /host/$build/fs/NDS/results
cp /NDS/*.txt /host/$build/fs/NDS/results
cp /mnt/*.txt /host/$build/fs/NDS/results
cp /NDS/*.csv /host/$build/fs/NDS/results
cp /NDS/*.xsl /host/$build/fs/NDS/results
cp /NDS/*.dtd /host/$build/fs/NDS/results
cp /NDS/*.gz /host/$build/fs/NDS/results
cp /NDS/*.log /host/$build/fs/NDS/results
cp /NDS/*.dat /host/$build/fs/NDS/results
cp /NDS/*.xslt /host/$build/fs/NDS/results
cp /NDS/*.css /host/$build/fs/NDS/results
cp /mnt/nds/data/*.db /host/$build/fs/NDS/results
if [ -e core.* ]
then
    echo "***WARNING: CORE FILE PRODUCED IN LAST TEST RUN.***"
    echo "Compressing core file with gzip and copying to fs/NDS/results."
    chmod 777 /root/core.*
    gzip core.*
    cp /root/core.* /host/$build/fs/NDS/results
fi
chown -R `ls -ld /host | awk '{print $3}'` /host/$build/fs/NDS/results
chmod 666 /host/$build/fs/NDS/results/*.*
sleep 1 # let the caller read any output from this script
reboot

