#!/bin/bash

FROM=$1
TO=$2

IEX_COMPS=(IEX_ELEMENTARYACTIONS IEX_GW_IAL IEX_INI_FILES IEX_PCAT_MODIFIER IEX_TESTS IEX_TOOLS IEX_SRC_FILES)

iex_comps_array_len=${#IEX_COMPS[*]}

mkdir $TO/IEX
i=0
while [ $i -lt $iex_comps_array_len ]
do
        cp -fR $FROM/${IEX_COMPS[$i]} $TO/IEX
        let i++
done