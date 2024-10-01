# Invoke build.py via B&I compatible makefile

.PHONY: all
all: install

.PHONY: clean
clean:
	@echo "make clean"
	@echo ./build.py --clean-action all --force --build-action none
	./build.py --clean-action all --force --build-action none
	pwd
	rm -rfdv ./**/NativeLibraries~
	rm -rfdv ./**/__pycache__

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


