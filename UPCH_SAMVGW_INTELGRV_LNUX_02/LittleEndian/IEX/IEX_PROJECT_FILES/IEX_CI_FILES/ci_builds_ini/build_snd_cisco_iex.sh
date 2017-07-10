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
	NDS_BUILD_ROOT=$BUILD_OUT_ROOT/UPCH_CISCOUGW1726_INTELGRV_LNUX_01/LittleEndian/epg/fs/NDS
	DRIVERS_ROOT=$BUILD_OUT_ROOT/UPCH_CISCOUGW1726_INTELGRV_LNUX_01/LittleEndian/drivers	
	FLASHTOOL_ROOT=$BUILD_OUT_ROOT/UPCH_CISCOUGW1726_INTELGRV_LNUX_01/LittleEndian/IEX/IEX_PROJECT_FILES/IEX_INI_FILES
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
	
	if [ $BUILD_TYPE == "nfs" ]; then
		exit 0
	fi
	
    ./BuildTools/makeImage.pl -project horizon -platform UPCH_CISCOUGW1726_INTELGRV_LNUX_01 -vobroot $VOB_ROOT -fast -release -auto  $NDS_BUILD_ROOT $DRIVERS_ROOT
	if [ "$?" -ne "0" ]; then
		echo "**** makeImage.pl FAILED ****"
		exit 1
	fi
	cp $FLASHTOOL_ROOT/SaObj $DRIVERS_ROOT/
	cd $DRIVERS_ROOT/
	chmod 777 SaObj
	./SaObj $DRIVERS_ROOT/MAIN.sao -G -v 0x100003c
	mv -f $DRIVERS_ROOT/MAIN.sao $DRIVERS_ROOT/bzImage
	
	
elif [ "$PLATFORM" == "istb" ]; then 
	NDS_BUILD_ROOT=$BUILD_OUT_ROOT/UPCH_SAMISTB_INTELGRV_LNUX_02/LittleEndian/epg/fs/NDS
	DRIVERS_ROOT=$BUILD_OUT_ROOT/UPCH_SAMISTB_INTELGRV_LNUX_02/LittleEndian/drivers	
	FLASHTOOL_ROOT=$BUILD_OUT_ROOT/UPCH_SAMISTB_INTELGRV_LNUX_02/LittleEndian/IEX/IEX_PROJECT_FILES/IEX_INI_FILES
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

	
	if [ $BUILD_TYPE == "nfs" ]; then
		exit 0
	fi
	
	./BuildTools/makeImage.pl -project horizon_istb -platform UPCH_SAMISTB_INTELGRV_LNUX_02 -vobroot $VOB_ROOT -fast -debug -auto -imagetype initramfs $NDS_BUILD_ROOT $DRIVERS_ROOT
	if [ "$?" -ne "0" ]; then
		echo "**** makeImage.pl FAILED ****"
		exit 1
	fi
	
	cp $FLASHTOOL_ROOT/sw_version_modifier.exe $DRIVERS_ROOT/
	cd $DRIVERS_ROOT/
	./sw_version_modifier.exe $DRIVERS_ROOT/UPC_SAM_ISTB.bin 16777276
	mv -f $DRIVERS_ROOT/UPC_SAM_ISTB.bin.version_0x100003c $DRIVERS_ROOT/bzImage
else
	NDS_BUILD_ROOT=$BUILD_OUT_ROOT/UPCH_SAMIPC_INTELGRV_LNUX_02/LittleEndian/epg/fs/NDS
	DRIVERS_ROOT=$BUILD_OUT_ROOT/UPCH_SAMIPC_INTELGRV_LNUX_02/LittleEndian/drivers
	FLASHTOOL_ROOT=$BUILD_OUT_ROOT/UPCH_SAMIPC_INTELGRV_LNUX_02/LittleEndian/IEX/IEX_PROJECT_FILES/IEX_INI_FILES

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
