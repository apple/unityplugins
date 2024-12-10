#!/bin/sh
# Repackage a release build .dmg image into one .tgz archive per plug-in.

# Example .dmg file name: 072-14189-A_GamePlugins_GamePlugins1A11.dmg
#                         ^part #^     ^project^  ^build version^            

script_basename=`basename "$0" .sh`
dmg=`realpath "$1"`
outdir=`realpath "$2"`

# check args
if [[ ! -f "$dmg" ]] || [[ ! -e "$outdir" ]] ; then
    echo "$script_basename: Repackage a release build .dmg image into one .tgz archive per plug-in."
    echo "Usage: $script_basename <dmg-file> <output-folder>"
    exit
fi

# extract build version suffix (e.g. 1A11)
build_suffix=`echo $dmg | sed -r 's/.*GamePlugins([0-9]+[A-Z]+[0-9]+)\.dmg/\1/'`
if [[ -z "$build_suffix" ]]; then
    echo "dmg file name doesn't seem to have a build number suffix (e.g. 1A11). Giving up."
    exit
fi

# create temp folder to mount image file
tmpdir=`mktemp -d`
if [ $? -ne 0 ]; then
    echo "$script_basename: Unable to create temporary folder. Exiting."
    exit 1
fi

# mount the dmg image
echo "Mounting $dmg in $tmpdir"
hdiutil attach "$dmg" -readonly -mountpoint "$tmpdir"
pushd "$tmpdir"

echo "Contents of $dmg:"
ls .

# repackage each plug-in
for dir in */
do
    echo "Repacking ${dir%/}."
    echo "tar -caf $outdir/${dir%/}.tgz -C ${dir%/} package"
    tar -caf "$outdir/${dir%/}.$build_suffix.tgz" -C "${dir%/}" package
done

popd

# clean up
diskutil eject "$tmpdir"
rmdir "$tmpdir"
