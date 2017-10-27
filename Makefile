# Copyright (c) Rotorz Limited. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root.

WIKI_REPOSITORY_URL := https://github.com/rotorz/unity3d-reorderable-list.wiki.git
DOCS_BOOK_YAML_PATH := ./docs/book.yaml


#
# Default Target
#
output: ;


#
# Target: Install npm
#
install-npm:

	npm install


#
# Target: Build Wiki
#
build-wiki: install-npm

	# Prepare temporary directory for wiki repository.
	rm -rf temp/wiki
	mkdir -p temp

	# Clone wiki repository.
	git clone $(WIKI_REPOSITORY_URL) temp/wiki

	# Clean previous build of documentation.
	npm run clean-wiki-output -- --input="$(DOCS_BOOK_YAML_PATH)" --output=temp/wiki
	# Build documentation.
	npm run build-wiki-output -- --input="$(DOCS_BOOK_YAML_PATH)" --output=temp/wiki

	# Commit and push changes to the wiki repository.
	git -C temp/wiki add . \
	  && git -C temp/wiki commit -m "Updated documentation." \
	  && git -C temp/wiki push \
	  ; true

	# Cleanup.
	rm -rf temp/wiki
	rmdir temp; true

	@echo ""
	@echo "Wiki updated from documentation files."
	@echo ""
