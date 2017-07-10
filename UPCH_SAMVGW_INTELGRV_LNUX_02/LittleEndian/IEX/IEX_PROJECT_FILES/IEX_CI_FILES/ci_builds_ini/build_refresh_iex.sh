#!/bin/bash

function usage(){
    echo ""
    echo "-----------------------------------------------------------------------"
    echo "Usage: build_iex.sh [-platform <gateway|client>] "  
	echo "                    [-build_type <nfs|flash>] "
	echo "                    [-vob_root <path to VOB root folder>] "
	echo "                    [-args <build command arguments>] "
	echo "                    [-build_out <path to the build output folder>] "
    echo "-----------------------------------------------------------------------"
    echo ""
}

# display the help
if [ $# -eq 0 ]; then
    usage
    exit 0
fi

# parse the arguments
while [ $# -gt 0 ]
do
    CURRENT_PARAMETER=$1 
	
    case "$CURRENT_PARAMETER" in
    -platform)
        shift;
        PLATFORM=$1
    ;;
	
	-build_type)
        shift;
        BUILD_TYPE=$1
    ;;
	
	-vob_root)
        shift;
        VOB_ROOT=$1
    ;;
	
	-o)
        shift;
        BUILD_OUT_ROOT=$1
    ;;
	
	-args)
        shift;
        BUILD_ARGS=$1
		echo "The build arguments are: " $BUILD_ARGS
    ;;	
	
    *)
        echo "unknown command... ${CURRENT_PARAMETER}"
		error="true"
    ;;
    esac
    shift
done

# validate arguments
if [ -z $PLATFORM ]; then
    echo "The platform type is missing."
    error="true"
fi

if [ -z $BUILD_TYPE ]; then
    echo "The build type is missing."
    error="true"
fi

if [ -z $BUILD_OUT_ROOT ]; then
    echo "The path to build output folder is missing."
    error="true"
fi

if [ -z $VOB_ROOT ]; then
    echo "The path to vob root is missing."
    error="true"
fi

if [ "${error}" == "true" ]; then
    usage    
    exit 1
fi

# build 
if [ "$PLATFORM" == "gateway" ]; then 
	NDS_BUILD_ROOT=$BUILD_OUT_ROOT/UPCH_SAMVGW_INTELGRV_LNUX_02/LittleEndian/epg/fs/NDS
	DRIVERS_ROOT=$BUILD_OUT_ROOT/UPCH_SAMVGW_INTELGRV_LNUX_02/LittleEndian/drivers	

	./patches.sh CI
	if [ "$?" -ne "0" ]; then
		echo "**** patches.sh FAILED ****"
		exit 1
	fi
	
	if [ $BUILD_TYPE == "nfs" ]; then
	    ./make.sh $BUILD_ARGS -path $BUILD_OUT_ROOT		
	else
		./make.sh $BUILD_ARGS -path $BUILD_OUT_ROOT
	fi
	if [ "$?" -ne "0" ]; then
		echo "**** make.sh FAILED ****"
		exit 1
	fi
	
	sed -i 's/enable="1"/enable="0"/g;s/DEBUG_ONBOOT="0"/DEBUG_ONBOOT="1"/g;s/"WARNING_OUTPUT_MASK"/"APP_OUTPUT_MASK"/g;s/"DEBUG_IEX" enable="0"/"DEBUG_IEX" enable="1"/g;s/"DEBUG_NAV_ZAPPING" enable="0"/"DEBUG_NAV_ZAPPING" enable="1"/g'  $NDS_BUILD_ROOT/resources/TracerConfigMap.xml
	sed -i 's/enable="1"/enable="0"/g;s/DEBUG_ONBOOT="0"/DEBUG_ONBOOT="1"/g;s/"ERROR_OUTPUT_MASK"/"IEX_OUTPUT_MASK"/g;s/"DEBUG_IEX" enable="0"/"DEBUG_IEX" enable="1"/g'  $NDS_BUILD_ROOT/resources/TracerConfigMap.xml
	sed -i 's/"enableWizard">true/"enableWizard">false/g' $NDS_BUILD_ROOT/resources/EPG_properties.xml
	sed -i 's/enableWizard="true"/enableWizard="false"/g' $NDS_BUILD_ROOT/resources/EPG_properties.xml

	
	sed -i 's/SOFT_UPDATING=TRUE/SOFT_UPDATING=FALSE/g' $NDS_BUILD_ROOT/config/spm.cfg
	sed -i 's/USE_TEST_VALUES=FALSE/USE_TEST_VALUES=TRUE/g' $NDS_BUILD_ROOT/config/spm.cfg
	cd $NDS_BUILD_ROOT/config/
	$BUILD_OUT_ROOT/UPCH_SAMVGW_INTELGRV_LNUX_02/LittleEndian/host/FusionConfigImport/FusionConfigImport  CFG2BIN ./ *.[cC][fF][gG]
	cd -
	
	if [ $BUILD_TYPE == "nfs" ]; then
		exit 0
	fi
	
	./BuildTools/makeImage.pl -project horizon -platform UPCH_SAMVGW_INTELGRV_LNUX_02 -vobroot $VOB_ROOT -fast -debug -auto -imagetype initramfs $NDS_BUILD_ROOT $DRIVERS_ROOT
	if [ "$?" -ne "0" ]; then
		echo "**** makeImage.pl FAILED ****"
		exit 1
	fi
	
	mv -f $DRIVERS_ROOT/UPC_SAM_VGW.bin $DRIVERS_ROOT/bzImage

elif [ "$PLATFORM" == "istb" ]; then 
	NDS_BUILD_ROOT=$BUILD_OUT_ROOT/UPCH_SAMISTB_INTELGRV_LNUX_02/LittleEndian/epg/fs/NDS
	DRIVERS_ROOT=$BUILD_OUT_ROOT/UPCH_SAMISTB_INTELGRV_LNUX_02/LittleEndian/drivers	

	./patches.sh CI
	if [ "$?" -ne "0" ]; then
		echo "**** patches.sh FAILED ****"
		exit 1
	fi
	
	if [ $BUILD_TYPE == "nfs" ]; then
	    ./make.sh $BUILD_ARGS -path $BUILD_OUT_ROOT		
	else
		./make.sh $BUILD_ARGS -path $BUILD_OUT_ROOT
	fi
	if [ "$?" -ne "0" ]; then
		echo "**** make.sh FAILED ****"
		exit 1
	fi
	
	sed -i 's/enable="1"/enable="0"/g;s/DEBUG_ONBOOT="0"/DEBUG_ONBOOT="1"/g;s/"WARNING_OUTPUT_MASK"/"APP_OUTPUT_MASK"/g;s/"DEBUG_IEX" enable="0"/"DEBUG_IEX" enable="1"/g'  $NDS_BUILD_ROOT/resources/TracerConfigMap.xml
	sed -i 's/enable="1"/enable="0"/g;s/DEBUG_ONBOOT="0"/DEBUG_ONBOOT="1"/g;s/"ERROR_OUTPUT_MASK"/"IEX_OUTPUT_MASK"/g;s/"DEBUG_IEX" enable="0"/"DEBUG_IEX" enable="1"/g'  $NDS_BUILD_ROOT/resources/TracerConfigMap.xml
	sed -i 's/enableWizard="true"/enableWizard="false"/g' $NDS_BUILD_ROOT/resources/EPG_properties.xml
	sed -i 's/"enableWizard">true/"enableWizard">false/g' $NDS_BUILD_ROOT/resources/EPG_properties.xml
	
	sed -i 's/SOFT_UPDATING=TRUE/SOFT_UPDATING=FALSE/g' $NDS_BUILD_ROOT/config/spm.cfg
	sed -i 's/USE_TEST_VALUES=FALSE/USE_TEST_VALUES=TRUE/g' $NDS_BUILD_ROOT/config/spm.cfg
	
	cd $NDS_BUILD_ROOT/config/
	$BUILD_OUT_ROOT/UPCH_SAMISTB_INTELGRV_LNUX_02/LittleEndian/host/FusionConfigImport/FusionConfigImport  CFG2BIN ./ *.[cC][fF][gG]
	cd -
	
	if [ $BUILD_TYPE == "nfs" ]; then
		exit 0
	fi
	
	./BuildTools/makeImage.pl -project horizon_istb -platform UPCH_SAMISTB_INTELGRV_LNUX_02 -vobroot $VOB_ROOT -fast -debug -auto -imagetype initramfs $NDS_BUILD_ROOT $DRIVERS_ROOT
	if [ "$?" -ne "0" ]; then
		echo "**** makeImage.pl FAILED ****"
		exit 1
	fi
	
	mv -f $DRIVERS_ROOT/UPC_SAM_ISTB.bin $DRIVERS_ROOT/bzImage
else
	NDS_BUILD_ROOT=$BUILD_OUT_ROOT/UPCH_SAMIPC_INTELGRV_LNUX_02/LittleEndian/epg/fs/NDS
	DRIVERS_ROOT=$BUILD_OUT_ROOT/UPCH_SAMIPC_INTELGRV_LNUX_02/LittleEndian/drivers

	./patches.sh CI;
	if [ "$?" -ne "0" ]; then
		echo "**** patches.sh FAILED ****"
		exit 1
	fi
	./make.sh $BUILD_ARGS -path $BUILD_OUT_ROOT
	if [ "$?" -ne "0" ]; then
		echo "**** make.sh FAILED ****"
		exit 1
	fi
 
	sed -i 's/enable="1"/enable="0"/g;s/DEBUG_ONBOOT="0"/DEBUG_ONBOOT="1"/g;s/"WARNING_OUTPUT_MASK"/"APP_OUTPUT_MASK"/g;s/"DEBUG_IEX" enable="0"/"DEBUG_IEX" enable="1"/g'  $NDS_BUILD_ROOT/resources/TracerConfigMap.xml
 	sed -i 's/enable="1"/enable="0"/g;s/DEBUG_ONBOOT="0"/DEBUG_ONBOOT="1"/g;s/"ERROR_OUTPUT_MASK"/"IEX_OUTPUT_MASK"/g;s/"DEBUG_IEX" enable="0"/"DEBUG_IEX" enable="1"/g'  $NDS_BUILD_ROOT/resources/TracerConfigMap.xml

	sed -i 's/SOFT_UPDATING=TRUE/SOFT_UPDATING=FALSE/g' $NDS_BUILD_ROOT/config/spm.cfg
	sed -i 's/USE_TEST_VALUES=FALSE/USE_TEST_VALUES=TRUE/g' $NDS_BUILD_ROOT/config/spm.cfg
	sed -i 's/DISCONNECT_NETWORK_IN_STANDBY=TRUE/DISCONNECT_NETWORK_IN_STANDBY=FALSE/g' $NDS_BUILD_ROOT/config/spm.cfg
		grep -n -A7 "# ncm_default_factory\.cfg" $NDS_BUILD_ROOT/start.sh | grep -v ncm_default_factory.cfg | sed 's/^\([0-9]*\)-.*/\1/g' | sort -r > linesToRemove
	while read p; do sed -i "${p} s/^/#/" $NDS_BUILD_ROOT/start.sh; done < linesToRemove
	
	grep -n -A7 "# ncm_default_factory\.cfg" start.sh | grep -v ncm_default_factory.cfg | sed 's/^\([0-9]*\)-.*/\1/g' | sort -r > linesToRemove
	while read p; do sed -i "${p} s/^/#/" start.sh; done < linesToRemove


	
	if [ $BUILD_TYPE == "nfs" ]; then
		exit 0
	fi
	
	./BuildTools/makeImage.pl -project horizon_ipc -platform UPCH_SAMIPC_INTELGRV_LNUX_02 -vobroot $VOB_ROOT -fast -debug -auto -imagetype initramfs $NDS_BUILD_ROOT $DRIVERS_ROOT
	if [ "$?" -ne "0" ]; then
		echo "**** makeImage.pl FAILED ****"
		exit 1
	fi
	
	mv -f $DRIVERS_ROOT/UPC_SAM_IPC.bin $DRIVERS_ROOT/bzImage	
fi
