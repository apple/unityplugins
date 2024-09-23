.PHONY: all
all:
	@echo "make all"
	./build.py

.PHONY: clean
clean:
	@echo "make clean: no-op"

.PHONY: test
test:
	@echo "make test: no-op"

.PHONY: installsrc
installsrc:
	@echo "make installsrc: ditto . $(SRCROOT)"
	ditto . $(SRCROOT)
