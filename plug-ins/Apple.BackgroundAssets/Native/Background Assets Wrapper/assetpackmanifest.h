//
//  assetpackmanifest.h
//  Background Assets Wrapper
//
//  Created by Gabriel Jacoby-Cooper on 4/20/26.
//  Copyright © 2026 Apple. All rights reserved.
//

#import <BackgroundAssetsWrapper/assetpack.h>

struct baw_assetpackmanifest_compat {
	size_t assetpackc;
	struct baw_assetpack *assetpackv;
};

union baw_assetpackmanifest {
	void *opaque;
	struct baw_assetpackmanifest_compat compat;
};

union baw_assetpackmanifest_res {
	union baw_assetpackmanifest success;
	struct baw_err failure;
};

void baw_assetpackmanifest_deinit(union baw_assetpackmanifest manifest);

struct baw_assetpack baw_assetpackmanifest_assetpack(union baw_assetpackmanifest manifest, const char *id);

struct baw_assetpack *baw_assetpackmanifest_assetpackv(union baw_assetpackmanifest manifest, size_t *assetpackc);

struct baw_assetpack *baw_assetpackmanifest_assetpackv_localized(union baw_assetpackmanifest manifest, size_t *assetpackc);

struct baw_assetpack *baw_assetpackmanifest_assetpackv_localized_lang(union baw_assetpackmanifest manifest, struct baw_lang lang, size_t *assetpackc);

void baw_assetpackmanifest_assetpackv_deinit(size_t assetpackc, struct baw_assetpack *assetpackv);

struct baw_lang baw_assetpackmanifest_lang_primary(union baw_assetpackmanifest manifest);

struct baw_lang baw_assetpackmanifest_lang_resolved(union baw_assetpackmanifest manifest);

struct baw_lang *baw_assetpackmanifest_langv(union baw_assetpackmanifest manifest, size_t *langc);

void baw_assetpackmanifest_langv_deinit(size_t langc, struct baw_lang *langv);
