# Invoke build.py via B&I compatible makefile

.PHONY: all
all: install

.PHONY: clean
clean:
	@echo "make clean"
	@echo ./build.py --clean-action all --force --build-action none
	./build.py --clean-action all --force --build-action none
	pwd
	zsh -o extended_glob -c 'rm -rfdv ./**/NativeLibraries~'
	zsh -o extended_glob -c 'rm -rfdv ./**/__pycache__'

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

.PHONY: install
install:
	@echo "make install"
	@echo ./build.py
	./build.py

	ditto $(SRCROOT)/Build $(DSTROOT)/Build

	ditto $(SRCROOT)/plug-ins/Apple.Core/Apple.Core_Unity/Assets/Apple.Core $(DSTROOT)/Assets/Apple.Core
	ditto $(SRCROOT)/plug-ins/Apple.GameController/Apple.GameController_Unity/Assets/Apple.GameController $(DSTROOT)/Assets/Apple.GameController
	ditto $(SRCROOT)/plug-ins/Apple.CoreHaptics/Apple.CoreHaptics_Unity/Assets/Apple.CoreHaptics $(DSTROOT)/Assets/Apple.CoreHaptics
	ditto $(SRCROOT)/plug-ins/Apple.Accessibility/Apple.Accessibility_Unity/Assets/Apple.Accessibility $(DSTROOT)/Assets/Apple.Accessibility
	ditto $(SRCROOT)/plug-ins/Apple.GameKit/Apple.GameKit_Unity/Assets/Apple.GameKit $(DSTROOT)/Assets/Apple.GameKit
	ditto $(SRCROOT)/plug-ins/Apple.PHASE/Apple.PHASE_Unity/Assets $(DSTROOT)/Assets/Apple.PHASE

