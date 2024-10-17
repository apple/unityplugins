# Invoke build.py via B&I compatible makefile

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
	
	# Set the destination folder names to match the .tgz archive names.
	@echo "Setting destination folder names from contents of $(SRCROOT)/Build."
	$(eval APPLE_CORE_FOLDER = $(shell ls $(SRCROOT)/Build | grep -i 'core-' | sed -E 's/\.tgz//'))
	$(eval APPLE_GAMECONTROLLER_FOLDER = $(shell ls $(SRCROOT)/Build | grep -i 'gamecontroller-' | sed -E 's/\.tgz//'))
	$(eval APPLE_COREHAPTICS_FOLDER = $(shell ls $(SRCROOT)/Build | grep -i 'corehaptics-' | sed -E 's/\.tgz//'))
	$(eval APPLE_ACCESSIBILITY_FOLDER = $(shell ls $(SRCROOT)/Build | grep -i 'accessibility-' | sed -E 's/\.tgz//'))
	$(eval APPLE_GAMEKIT_FOLDER = $(shell ls $(SRCROOT)/Build | grep -i 'gamekit-' | sed -E 's/\.tgz//'))
	$(eval APPLE_PHASE_FOLDER = $(shell ls $(SRCROOT)/Build | grep -i 'phase-' | sed -E 's/\.tgz//'))

	@echo "Copying package folder trees."
	ditto $(SRCROOT)/plug-ins/Apple.Core/Apple.Core_Unity/Assets/Apple.Core $(DSTROOT)/$(APPLE_CORE_FOLDER)/package
	ditto $(SRCROOT)/plug-ins/Apple.GameController/Apple.GameController_Unity/Assets/Apple.GameController $(DSTROOT)/$(APPLE_GAMECONTROLLER_FOLDER)/package
	ditto $(SRCROOT)/plug-ins/Apple.CoreHaptics/Apple.CoreHaptics_Unity/Assets/Apple.CoreHaptics $(DSTROOT)/$(APPLE_COREHAPTICS_FOLDER)/package
	ditto $(SRCROOT)/plug-ins/Apple.Accessibility/Apple.Accessibility_Unity/Assets/Apple.Accessibility $(DSTROOT)/$(APPLE_ACCESSIBILITY_FOLDER)/package
	ditto $(SRCROOT)/plug-ins/Apple.GameKit/Apple.GameKit_Unity/Assets/Apple.GameKit $(DSTROOT)/$(APPLE_GAMEKIT_FOLDER)/package
	ditto $(SRCROOT)/plug-ins/Apple.PHASE/Apple.PHASE_Unity/Assets $(DSTROOT)/$(APPLE_PHASE_FOLDER)/package

