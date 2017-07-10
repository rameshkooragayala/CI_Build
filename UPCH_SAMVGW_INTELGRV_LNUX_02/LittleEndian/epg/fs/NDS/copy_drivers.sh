#!/bin/bash

###################################################################################
# This script is run tp copy drivers/scripts/utils required to successfully
# run an INTEL GRoveland STB from a clearcase view structure to another directory.
#
# CHANGE HISTORY
#
# AH 27/11/07 Initial creation from CMS make script to support initial release of unified script.
# AH 05/12/07 Update to remove dependency on KERN* and SYSTEM_BIN loadrules.
# BT 13/08/2010 First version for UPC Intel Groveland chip (Samsung board -Gateway-)
###################################################################################

view_root=$1; shift;
output_root=$1; shift;

echo "Copying Intel GRV driver files, please wait..."

# we copy the release xtvfs_utils, or it is too verbose

driver_files="
FUSIONOS_10/BLD_UPCH_SAMVGW_INTELGRV_LNUX_02/platform_cfg/linux/LittleEndian/release_dbg/BootImage/bzImage.initramfs
FUSIONOS_10/BLD_UPCH_SAMVGW_INTELGRV_LNUX_02/platform_cfg/linux/LittleEndian/release_dbg/BootImage/initrd.gz
FUSIONOS_10/BLD_UPCH_SAMVGW_INTELGRV_LNUX_02/platform_cfg/linux/LittleEndian/release_dbg/lib_NDS/libpoll.so
FUSIONOS_10/BLD_UPCH_SAMVGW_INTELGRV_LNUX_02/platform_cfg/linux/LittleEndian/release_dbg/lib_NDS/libprio_range.so
FUSIONOS_10/BLD_UPCH_SAMVGW_INTELGRV_LNUX_02/platform_cfg/linux/LittleEndian/release_dbg/Scripts/firewall_clean.sh
FUSIONOS_10/BLD_UPCH_SAMVGW_INTELGRV_LNUX_02/platform_cfg/linux/LittleEndian/release_dbg/Scripts/firewall.sh
FUSIONOS_10/BLD_UPCH_SAMVGW_INTELGRV_LNUX_02/platform_cfg/linux/LittleEndian/release_dbg/Scripts/open_firewall_port.sh
FUSIONOS_10/BLD_UPCH_SAMVGW_INTELGRV_LNUX_02/platform_cfg/linux/LittleEndian/release_dbg/Scripts/firewall-ipv6.sh
FUSIONOS_10/BLD_UPCH_SAMVGW_INTELGRV_LNUX_02/platform_cfg/linux/LittleEndian/release_dbg/Modules_NDS/xtvfs.ko
FUSIONOS_10/BLD_UPCH_SAMVGW_INTELGRV_LNUX_02/platform_cfg/linux/LittleEndian/release_dbg/bin_NDS/xtvfs_format
FUSIONOS_10/BLD_UPCH_SAMVGW_INTELGRV_LNUX_02/platform_cfg/linux/LittleEndian/release/bin_NDS/xtvfs_utils
FUSIONOS_10/BLD_UPCH_SAMVGW_INTELGRV_LNUX_02/platform_cfg/linux/LittleEndian/release_dbg/lib_NDS/cdi_tracker.so
FUSIONOS_10/BLD_UPCH_SAMVGW_INTELGRV_LNUX_02/platform_cfg/linux/LittleEndian/release_dbg/lib_NDS/helper_tracker.so
FUSIONOS_10/BLD_UPCH_SAMVGW_INTELGRV_LNUX_02/platform_cfg/linux/LittleEndian/release_dbg/bin_NDS/cdit_server
FUSIONOS_10/BLD_UPCH_SAMVGW_INTELGRV_LNUX_02/platform_cfg/linux/LittleEndian/release_dbg/bin_NDS/rf4ce_diag
FUSIONOS_10/BLD_UPCH_SAMVGW_INTELGRV_LNUX_02/platform_cfg/linux/LittleEndian/release_dbg/config/fusion_os_cdi_labels.cfg
FUSIONOS_10/BLD_UPCH_SAMVGW_INTELGRV_LNUX_02/platform_cfg/linux/LittleEndian/release_dbg/config/fusion_os_storage_labels.cfg
FUSIONOS_10/BLD_UPCH_SAMVGW_INTELGRV_LNUX_02/platform_cfg/linux/LittleEndian/release_dbg/config/upc_dal_osy.cfg
FUSIONOS_10/BLD_UPCH_SAMVGW_INTELGRV_LNUX_02/platform_cfg/linux/tools/fakeroot/rhel5/fakeroot
FUSIONOS_10/BLD_UPCH_SAMVGW_INTELGRV_LNUX_02/platform_cfg/linux/tools/fakeroot/rhel5/libfakeroot.so
FUSIONOS_10/BLD_UPCH_SAMVGW_INTELGRV_LNUX_02/platform_cfg/linux/LittleEndian/debug/lib_NDS/bslcomm.so
FUSIONOS_10/BLD_UPCH_SAMVGW_INTELGRV_LNUX_02/platform_cfg/linux/LittleEndian/debug/lib_NDS/libResolveRefresh.so
"
####driver_files_ko_path=FUSIONOS_3/BLD_UPCH_SAMVGW_INTELGRV_LNUX_02/platform_cfg/linux/LittleEndian/release_dbg/Modules


# Copy files from local view, creating the same directory structure below $output_root as
# there is in the view (i.e. as in SYSTEM_COMPOSITE).

for driver_file in $driver_files
do
    input_file=$view_root/$driver_file
    output_dir=$output_root/drivers
    output_file=$output_dir/`basename $input_file`

    if [ ! -e $input_file ]
    then
        echo "$input_file not found"
        echo "Script has failed"
        #exit -1 # (don't want to break builds that don't have right load rules - yet)
    fi

    mkdir -p $output_dir
    if [ $? -ne 0 ]
    then
        "Unable to create directory $output_dir"
        echo "Script has failed"
        #exit -1 # (don't want to break builds that don't have right load rules - yet)
    fi

    cp -pf $input_file $output_file
    if [ $? -ne 0 ]
    then
        "Unable to copy file $input_file"
        echo "Script has failed"
        #exit -1 # (don't want to break builds that don't have right load rules - yet)
    fi

    chmod +rwx $output_file
done

# Copy all .ko files from local view, creating the same directory structure below $output_root as
# there is in the view (i.e. as in SYSTEM_COMPOSITE).
#cp -prf $view_root/$driver_files_ko_path/* $output_root/drivers/
