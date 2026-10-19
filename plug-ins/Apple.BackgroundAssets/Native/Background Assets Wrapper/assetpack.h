//
//  assetpack.h
//  Background Assets Wrapper
//
//  Created by Gabriel Jacoby-Cooper on 4/20/26.
//  Copyright © 2026 Apple. All rights reserved.
//

#import <BackgroundAssetsWrapper/err.h>
#import <BackgroundAssetsWrapper/lang.h>

#define baw_assetpack_status_downloadavailable(status)	status & 0b0000001
#define baw_assetpack_status_updateavailable(status)	status & 0b0000010
#define baw_assetpack_status_uptodate(status)			status & 0b0000100
#define baw_assetpack_status_outofdate(status)			status & 0b0001000
#define baw_assetpack_status_obsolete(status)			status & 0b0010000
#define baw_assetpack_status_downloading(status)		status & 0b0100000
#define baw_assetpack_status_downloaded(status)			status & 0b1000000

union baw_assetpack_status_res {
	unsigned char success;
	struct baw_err failure;
};

struct baw_assetpack {
	void *impl;
};

void baw_assetpack_deinit(struct baw_assetpack assetpack);

bool baw_assetpack_is_nonnull(struct baw_assetpack assetpack);

char *baw_assetpack_id(struct baw_assetpack assetpack);

void baw_assetpack_id_deinit(char *id);

size_t baw_assetpack_downloadsize(struct baw_assetpack assetpack);

unsigned short int baw_assetpack_version(struct baw_assetpack assetpack);

struct baw_lang baw_assetpack_lang(struct baw_assetpack assetpack);

void baw_assetpack_userinfo(struct baw_assetpack assetpack, void *buf, size_t *len);
