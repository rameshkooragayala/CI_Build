PARENT COMPONENT = diag_log
    diag_ipc_client

PARENT COMPONENT = diag_ipc_client
    diag_cmn
    diag_hlp_fundamentals

PARENT COMPONENT = diag_cmn
    diag_hlp_debug

PARENT COMPONENT = memman
    memman_hlp

PARENT COMPONENT = IPC_CLIENT
    IPC_WRAPPER

PARENT COMPONENT = IPC_WRAPPER
    IPCTL

PARENT COMPONENT = systemlang
    systemconfig

PARENT COMPONENT = systemutil
    systemutil_common
    systemutil_hash
    systemutil_dlist
    systemutil_rbtree
    systemutil_thread
    systemutil_handle
    systemutil_libc
    systemutil_toolbox
    systemutil_xml
    systemutil_nsa
    systemutil_json
    systemutil_sax

PARENT COMPONENT = systemutil_xml
    ixml

PARENT COMPONENT = systemutil_json
    cJSON

PARENT COMPONENT = systemutil_sax
    su_expat

PARENT COMPONENT = camm_ipc_client
    camm_util

PARENT COMPONENT = IPC_SERVER
    IPC_WRAPPER

PARENT COMPONENT = http
    http_api
    http_curl
    http_upset
    rtsp

PARENT COMPONENT = nck
    nck_common
    /local/cruisecontrol/SYSTEM_BIN/views/MANIFESTS.upc.master/FUSION_SYSTEM_INTEGRATION/UPC_INTEGRATION/build/components/security_services/nagra_ca_kernel/src/cak/lib/UPCH_SAMVGW_INTELGRV_LNUX_02/release/cak_cl_test.a
    /local/cruisecontrol/SYSTEM_BIN/views/MANIFESTS.upc.master/FUSION_SYSTEM_INTEGRATION/UPC_INTEGRATION/build/components/security_services/nagra_ca_kernel/src/cak/lib/UPCH_SAMVGW_INTELGRV_LNUX_02/release/cak_cl.a
    /local/cruisecontrol/SYSTEM_BIN/views/MANIFESTS.upc.master/FUSION_SYSTEM_INTEGRATION/UPC_INTEGRATION/build/components/security_services/nagra_ca_kernel/src/cak/lib/UPCH_SAMVGW_INTELGRV_LNUX_02/release/dvl_std_test.a
    /local/cruisecontrol/SYSTEM_BIN/views/MANIFESTS.upc.master/FUSION_SYSTEM_INTEGRATION/UPC_INTEGRATION/build/components/security_services/nagra_ca_kernel/src/cak/lib/UPCH_SAMVGW_INTELGRV_LNUX_02/release/dvl_std.a

PARENT COMPONENT = ngc
    nck_common

PARENT COMPONENT = carange
    xosglue

PARENT COMPONENT = SYSINIT_SERVER
    SYSINIT_CLIENT
    SYSINIT_MW_COMMON
    SYSINIT_MW_COMMON

PARENT COMPONENT = SSM_CLIENT_UPNP
    SSM_CLIENT

PARENT COMPONENT = ncm_server
    ncm_common

PARENT COMPONENT = dhcp
    dhcp_api
    dhcp_iscdhcp

PARENT COMPONENT = SSM_SERVER
    SSM_UPNP_CMC

PARENT COMPONENT = darwin_scm_mscp
    darwin_scm_common

PARENT COMPONENT = ncm_client
    ncm_common

PARENT COMPONENT = httpserver
    httpserver_api
    httpserver_lighttpd
    httpserver_libc

PARENT COMPONENT = cmdc
    cmdc_response

PARENT COMPONENT = cmdc_response
    thirdparty_CJSON

PARENT COMPONENT = spc_client
    spc_common

PARENT COMPONENT = sim_sql_lifecycle_controller
    sim_mpeg_protocol
    sim_dvb_protocol
    sim_nds_protocol
    sim_priv1_protocol
    sim_dtg_protocol
    sim_sql_session_controller
    sim_sql_database_client
    sim_sql_query_handler
    sim_toolbox_src
    sim_transcoding
    sim_thread_create_library
    sim_utils_profile_library
    sim_browser
    sim_util_descriptor
    sim_flag_mapper
    sim_util_psi
    sim_svc_mapping
    sim_utils_systemobject
    sim_pi_dvb_sql
    binxml

PARENT COMPONENT = sim_sql_session_controller
    sim_ipc_comms
    sim_rrp_src

PARENT COMPONENT = sim_sql_database_client
    sim_sql_database_client_addon_chnum
    sim_sql_database_client_addon_persistence

PARENT COMPONENT = sim_pi_dvb_sql
    sim_mso
    sim_util_psi
    sim_pi_service_candidates
    sim_pi_service_eitpf_cache
    sim_pi_service_file
    sim_pi_service_mc_list
    sim_pi_service_mcmonitor
    sim_pi_service_mctsd
    sim_pi_service_media_connections
    sim_pi_service_mhsi
    sim_pi_service_object_collective_updates
    sim_pi_service_psi_cache
    sim_pi_service_network
    sim_pi_service_sdt_acquisition
    sim_pi_service_sectionstofile
    sim_pi_service_service_plan
    sim_pi_service_sfm_session
    sim_pi_service_tablecache
    sim_pi_service_tms
    sim_pi_service_tsmonitor

PARENT COMPONENT = sim_client_si
    sim_rrp_src
    sim_toolbox_src
    sim_ipc_comms
    sim_utils_category

PARENT COMPONENT = darwin_prefs_srv
    darwin_prefs_srv_s2c
    darwin_prefs_common

PARENT COMPONENT = spm_server
    spm_common
    spm_server_rmf
    spm_server_client_interface_module
    spm_server_xsi
    spm_server_emm_swdl_module
    spm_server_update
    spm_server_ca
    spm_server_ca_vgc
    spm_server_state_machine
    spm_server_system
    spm_server_network
    spm_server_fem_module
    spm_server_carousel
    spm_server_cdi_bslcomm
    spm_server_sim_module
    spm_server_statman
    spm_server_dvb
    spm_server_software_update
    spm_server_ps_module
    spm_server_setup_data
    spm_server_homenet
    spm_server_auth
    spm_server_software_update_upc

PARENT COMPONENT = spm_client
    spm_common
    spm_client_update

PARENT COMPONENT = darwin_srm
    darwin_srm_main
    darwin_srm_job
    darwin_srm_utils
    darwin_srm_purchase
    darwin_srm_expiry
    darwin_srm_history
    darwin_srm_ei

PARENT COMPONENT = darwin_sbm_server
    darwin_sbm_common

PARENT COMPONENT = darwin_sbm_client
    darwin_sbm_common

PARENT COMPONENT = darwin_uam
    egen
    darwin_uam_main
    darwin_uam_utils
    darwin_uam_dcm

PARENT COMPONENT = egen
    egen_config
    egen_framework
    egen_portinglayer
    egen_log_output
    egen_FUSION
    egen_live
    egen_view
    egen_play
    egen_clients_mngr
    generic_api
    egen_clients_mngr
    dc_framework
    dc_live
    dc_view
    dc_iapp
    dc_signal
    dc_interface
    dc_filesys
    dc_ftm
    dc_play
    dc_dvr
    dc_sys_info
    dc_headers
    dc_generic

PARENT COMPONENT = gfx
    gfx_config
    gfx_blitter
    gfx_clut
    gfx_clutmatcher
    gfx_font
    gfx_gif
    gfx_gif_interface
    gfx_iframe
    gfx_image
    gfx_input
    gfx_icon
    gfx_jpeg_interface
    gfx_mixer
    gfx_painter
    gfx_png_interface
    gfx_region
    gfx_scaler
    gfx_shape
    gfx_spansink
    gfx_surface
    gfx_scene
    gfx_mde_library
    gfx_photo
    gfx_egl

PARENT COMPONENT = gfx_mde_library

PARENT COMPONENT = freetype2
    freetype2_autofit
    freetype2_base
    freetype2_cache
    freetype2_gxvalid
    freetype2_gzip
    freetype2_lzw
    freetype2_otvalid
    freetype2_psaux
    freetype2_pshinter
    freetype2_psnames
    freetype2_raster
    freetype2_smooth
    freetype2_fusion_ftsystem
    freetype2_truetype
    freetype2_type1
    freetype2_cff
    freetype2_cid
    freetype2_pfr
    freetype2_type42
    freetype2_winfonts
    freetype2_pcf
    freetype2_bdf
    freetype2_sfnt

PARENT COMPONENT = fdm_svr
    fdm_common
    fdm_svr_misc
    fdm_common_plugin
    fdm_svr_dsmcc

PARENT COMPONENT = fdm_common
    fdm_common_msg

PARENT COMPONENT = fdm_common_plugin
    fdm_mhc_lists
    fdm_mhc_util

PARENT COMPONENT = fdm_fs
    fdm_common
    fdm_api_core
    fdm_fs_uland
    fdm_fs_dsmcc
    fdm_api_mhc

PARENT COMPONENT = fdm_api_core
    fdm_mhc_util

PARENT COMPONENT = rmf_client
    rmf_common

PARENT COMPONENT = rmf_server
    rmf_common

PARENT COMPONENT = vrm_server
    vrm_server_manager
    vrm_server_engine
    vrm_server_panel
    vrm_server_rpc
    vrm_common

PARENT COMPONENT = vrm_common
    vrm_common_misc
    vrm_common_rpc
    vrm_common_interface
    vrm_common_pbag
    vrm_common_dbag

PARENT COMPONENT = pcat_common
    pcat_common_misc
    pcat_common_interface
    pcat_common_rpc
    pcatp_common_interface

PARENT COMPONENT = pcat_server
    pcat_server_manager
    pcat_server_rpc
    pcat_server_dbview

PARENT COMPONENT = dbengine
    /local/cruisecontrol/SYSTEM_BIN/views/MANIFESTS.upc.master/FUSION_SYSTEM_INTEGRATION/UPC_INTEGRATION/build/components/information_services/dbengine/src/Interface/ICU3/source/build_linux/lib/libicudata.a
    /local/cruisecontrol/SYSTEM_BIN/views/MANIFESTS.upc.master/FUSION_SYSTEM_INTEGRATION/UPC_INTEGRATION/build/components/information_services/dbengine/src/Interface/ICU3/source/build_linux/lib/libicui18n.a
    /local/cruisecontrol/SYSTEM_BIN/views/MANIFESTS.upc.master/FUSION_SYSTEM_INTEGRATION/UPC_INTEGRATION/build/components/information_services/dbengine/src/Interface/ICU3/source/build_linux/lib/libicuuc.a
    dbengine_misc
    sqlite3
    dbe_interface

PARENT COMPONENT = camm_ipc_server
    camm_util

PARENT COMPONENT = msm
    mediastreamer
    metadata
    rasphigh
    mfs
    fs_fos
    msm_shared
    import

PARENT COMPONENT = msm_shared
    msm_control
    xi64
    xosglue
    xdebug2diag
    msm_acq
    msm_export_import_nff
    nff
    dli

PARENT COMPONENT = msm_export_import_nff
    msm_import_nff
    msm_export_nff

PARENT COMPONENT = upnp
    upnp_common
    upnp_lib

PARENT COMPONENT = rnc_server
    rnc_common
    rnc_xcl
    rnc_gen
    rnc_fusion
    asn1_tms_msg

PARENT COMPONENT = darwin_sdpc_server
    sdpclib
    sdpclib_common
    sdpc_ipc
    sdpclib
    sdpclib_common
    sdpc_ipc

PARENT COMPONENT = darwin_sdpc_client
    sdpclib_common
    sdpc_ipc
    sdpclib_common
    sdpc_ipc
    sdpclib_client

PARENT COMPONENT = atm
    atmlib

PARENT COMPONENT = atmlib
    atm_wrapper

PARENT COMPONENT = darwin_scm_server
    darwin_scm_common

PARENT COMPONENT = panorama_agent_server
    panorama_upc
    panorama_gen
    panorama_fusion

PARENT COMPONENT = panorama_gen
    panorama_tr069
    jungo_pkg

PARENT COMPONENT = diag_svr
    diag_ipc_server

PARENT COMPONENT = diag_ipc_server
    diag_cmn
    diag_hlp_fundamentals

PARENT COMPONENT = darwin_scm_client
    darwin_scm_common

PARENT COMPONENT = vrm_client
    vrm_client_rpc
    vrm_common

PARENT COMPONENT = pcat_client
    pcat_client_rpc

PARENT COMPONENT = cappres
    cappres_cwb
    cappres_common
    cappres_config
    cappres_event_buffer
    cappres_ttx_player
    cappres_ttx_decoder
    cappres_horizontal_subt_player
    cappres_horizontal_subt_decoder
    cappres_dvb_player
    cappres_dvb_decoder
    cappres_magazine_magazine
    cappres_magazine_driver
    cappres_ttx_idp

PARENT COMPONENT = darwin_prefs_cli
    darwin_prefs_common

PARENT COMPONENT = darwin_search
    gsa

PARENT COMPONENT = darwin_fas
    mhw_util
    mhw_common
    mhw_gfx
    mhw_service_navigation
    mhw_service_guide
    mhw_service_sm
    mhw_service_booking
    mhw_service_booking_pvr
    mhw_service_application
    mhw_service_common
    mhw_platform
    mhw_platform_io
    mhw_platform_network
    mhw_platform_update
    mhw_platform_settings
    mhw_platform_control
    mhw_platform_ca
    mhw_platform_log
    mhw_platform_messages
    mhw_platform_pvr
    mhw_platform_frontpanel
    mhw_platform_peripherals
    mhw_device
    mhw_storage
    mhw_storage_pvr
    mhw_content
    mhw_content_booking
    mhw_content_booking_pvr
    mhw_content_downloading
    mhw_content_playing
    mhw_content_playing_pvr
    mhw_content_playing_remote
    mhw_search
    mhw_user
    mhw_content_scanning
    mhw_content_searching
    mhw_content_metadata
    mhw_store
    mhw_universal_query
    mhw_universal_collapse
    mhw_universal_event
    ext

PARENT COMPONENT = rnc_client
    rnc_common

PARENT COMPONENT = webkitproxy
    webkitproxy_rpc

PARENT COMPONENT = wis
    wis_core
    wis_platform

PARENT COMPONENT = wis_core
    wis_core_proxy

PARENT COMPONENT = wis_platform
    wis_platform_fusion
    wis_platform_common

PARENT COMPONENT = wis_platform_fusion
    wis_platform_fusion_server
    wis_platform_fusion_utils

PARENT COMPONENT = darwin_aflproxy
    darwin_aflproxy_bindings_AS3

PARENT COMPONENT = darwin_aflproxy_bindings_AS3
    darwin_aflproxy_fas_adapter

PARENT COMPONENT = darwin_aflproxy_fas_adapter

PARENT COMPONENT = afl
    /local/cruisecontrol/SYSTEM_BIN/views/MANIFESTS.upc.master/FUSION_SYSTEM_INTEGRATION/UPC_INTEGRATION/build/components/application_engines_rte/afl_engine_lib/lib/UPCH_SAMVGW_INTELGRV_LNUX_02/linux/LittleEndian/release_dbg/libafl_engine.a

PARENT COMPONENT = webkitmonitor
    webkitproxy_rpc

