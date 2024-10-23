#!/bin/sh
# Repackage a release build .dmg image into one .tgz archive per plug-in.

script_basename=$(basename "$0")
dmg=$1
outdir=$2

# check args
if [[ ! -f "$dmg" ]] || [[ ! -e "$outdir" ]] ; then
    echo "$script_basename: Repackage a release build .dmg image into one .tgz archive per plug-in."
    echo "Usage: $script_basename <dmg-file> <output-folder>"
    exit
fi

# create temp folder to mount image file
tmpdir=`mktemp -d`
if [ $? -ne 0 ]; then
    echo "$script_basename: Unable to create temporary folder. Exiting."
    exit 1
fi

# mount the dmg image
hdiutil attach "$dmg" -readonly -mountpoint "$tmpdir"
pushd "$tmpdir"

# repackage each plug-in
for dir in */
do
    echo "Repacking ${dir%/}."
    tar -caf "$outdir/${dir%/}.tgz" -C "${dir%/}" package
done

popd

# clean up
diskutil eject "$tmpdir"
rmdir "$tmpdir"
