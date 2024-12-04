# Invoke build.py via B&I compatible makefile
# More details here: https://quip-apple.com/WR2vAGY9FLCD

# To test locally via buildit:
# sudo xbs buildit -project GamePlugins -update PrevailingGamePlugins .

.PHONY: all
all: install

.PHONY: clean
clean:
	@echo "make clean"
	@echo ./build.py --no-color --clean-action all --force --build-action none
	./build.py --no-color --clean-action all --force --build-action none
	pwd
	zsh -o extended_glob -c 'rm -rfdv ./**/NativeLibraries~(N)'
	zsh -o extended_glob -c 'rm -rfdv ./**/__pycache__(N)'

.PHONY: test
test:
	@echo "make test: no-op"

.PHONY: installsrc
installsrc:
	@echo "make installsrc: ditto . $(SRCROOT)"
	ditto . $(SRCROOT)

.PHONY: installhdrs
installhdrs:
	@echo "make installhdrs: no-op"

.PHONY: build
build:
	@echo "make build"
	@echo ./build.py --no-color
	./build.py --no-color

.PHONY: install
install: build
	@echo "make install"
	@echo "Unpacking plug-in .tgz archives to allow signing of contents by B&I."
	$(SRCROOT)/scripts/shell/unpack-tgzs.sh $(SRCROOT)/Build $(DSTROOT)

