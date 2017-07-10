#! /usr/bin/perl

#use strict;
#use Switch;
use Getopt::Long;

my $dir = "";
my $help;
my $slash = "\\";;
my $op_folder;
my $result;
my $mhsi_config;
my $spm_config;
my $path;
my $project;

system("cls");

#Fetch Options from command line
$result = GetOptions (  "p=s"   => \$project,		# Project Name
						"f=s"   => \$op_folder,     # build folder path.
						"h"     => \$help);        	# display help
						



# print Help text
if($help)
{
	help();
}

if ((!$op_folder) || (!$project))
{
	if (!$op_folder)
	{
		print "OP folder path not specified.\n";
	}
	else
	{
		print "Project Name not specified.\n";
	}
	exit 0;
}
else
{
	$IEX_path = $op_folder . $slash . "IEX";
	$fs_path = $op_folder . $slash . "fs\\NDS";
	$config_path = $fs_path . $slash . "config";
}

$mhsi_config = $config_path . $slash . "mhsi.cfg";
$spm_config = $config_path . $slash . "spm.cfg";
$start_sh = $fs_path . $slash . "start.sh";

# printing user arguments
print "\n******************************************************************\n";
print "Project Name	: $project\n";
print "O/P path	: $op_folder\n";
print "FS Path		: $fs_path\n";
print "Config Path	: $config_path\n";
print "IEX Path	: $IEX_path\n\n";
print "Start.sh	: $start_sh\n";
print "MHSI Config	: $mhsi_config\n";
print "SPM Config	: $spm_config\n";
print "*********************************\n";


sub help()
{
	print "\t**************************************** USAGE ****************************************\n\n";
	print "\tThis utility expects 2 arguments -\n\t  1. Project Name\t[-p]\t[CANALD / GET etc.]\n\t  2. O/P folder.\t[-f]\t[Build Output path]\n\n";
	print "\tThis utility assumes following -\n";
	print "\t  1. IEX Path\t\t: \"c:\\cruisecontrol\\SYSTEM_BIN\\ci_builds\\<IEX Thread>\\IEX\"\n";
	print "\t  2. Image Path\t\t: \"c:\\cruisecontrol\\SYSTEM_BIN\\ci_builds\\<IEX Thread>\\fs\\NDS\"\n";
	print "\t  3. Config Path\t: \"c:\\cruisecontrol\\SYSTEM_BIN\\ci_builds\\<IEX Thread>\\fs\\NDS\\config\"\n\n";
	print "\tThis utility does following operations -\n";
	print "\t 1. Copy Stream Sync utility from \\IEX\\IEX_TOOLS to \\fs\\NDS folder\n";
	print "\t 2. Modify \\fs\\NDS\\start.sh by addig stream sync command before launching application.\n";
	print "\n\t 3. GET specific -\n";
	print "\t  3a. Modify tuning parameters in \\fs\\NDS\\config\\spm.cfg\n";
	print "\t  3b. Modify MHC_Application_Name in \\fs\\NDS\\config\\mhsi.cfg\n";
	print "\n\t***************************************************************************************\n";
	
	exit 0;
}

sub modify_startup()
{
	$run_command = "Launching application...";
	$stream_sync = "\\host\\fs\\NDS\\streamutil_$project $project \@\@STREAM_START_TIME\@\@ \@\@STREAM_END_TIME\@\@";
	$src_dir = $IEX_path . $slash . "IEX_TOOLS" . $slash . "StreamSync_utilities\\$project";
	$dest_dir = $fs_path . $slash . "config";
	$copy_filename = $src_dir . $slash . "streamutil_$project";
	print ">>>> copying stream utility to /NDS//config\n";
	print "copy $copy_filename $fs_path \n";
	system("copy $copy_filename $fs_path");
	$copy_filename = $src_dir . $slash . "dms.cfg";
	system("copy $copy_filename $fs_path");
	print ">>>> modifying start.sh\n";
	open (FILE, " < $start_sh") or die "can't open $start_sh\n";
	@lines = <FILE>;
	close FILE;
	$length = @lines;
	open (FILE, " > $start_sh") or die "can't open $mhsi_config\n";

	for ($loop=0;$loop < $length ; $loop++)
	{
		print FILE $lines[$loop];
		if ($lines[$loop] =~ /$run_command/)
		{
			print FILE "  $stream_sync\n";
		}
	}
	close FILE;	
}

if ($project eq "CDIGITAL")
{
	modify_startup();
}

if ($project eq "GET")
{
	modify_startup();
}


