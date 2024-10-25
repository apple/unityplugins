#!/bin/sh
# Unpack plug-in .tgz archives to allow signing of contents."
# Usage: unpack-tgzs <srcdir> <outdir>

script_basename=`basename "$0" .sh`
tgzdir=$1
outdir=$2

# check args
if [[ ! -e "$tgzdir" ]] || [[ ! -e "$outdir" ]] ; then
    echo "$script_basename: Unpack plug-in .tgz archives to allow signing of contents."
    echo "Usage: $script_basename <tgz-folder> <output-folder>"
    exit
fi

# unpack each plug-in
for tgz in $tgzdir/*.tgz
do
    tgz_basename=`basename "$tgz" .tgz`
    echo "Unpacking $tgz to $outdir/$tgz_basename."
    mkdir "$outdir/$tgz_basename"
    tar -xf "$tgz" --cd "$outdir/$tgz_basename"
done	
