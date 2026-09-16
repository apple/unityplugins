//
//  assetpackmanager.h
//  Background Assets Wrapper
//
//  Created by Gabriel Jacoby-Cooper on 4/20/26.
//  Copyright © 2026 Apple. All rights reserved.
//

#import <BackgroundAssetsWrapper/assetpackmanifest.h>
#import <BackgroundAssetsWrapper/res.h>

enum baw_assetpackmanager_downloadstatusupdate_kind {
	baw_assetpackmanager_downloadstatusupdate_kind_began,
	baw_assetpackmanager_downloadstatusupdate_kind_paused,
	baw_assetpackmanager_downloadstatusupdate_kind_downloading,
	baw_assetpackmanager_downloadstatusupdate_kind_finished,
	baw_assetpackmanager_downloadstatusupdate_kind_failed
};

struct baw_assetpackmanager_downloadstatusupdate {
	enum baw_assetpackmanager_downloadstatusupdate_kind kind;
	struct baw_assetpack assetpack;
	union {
		double progress;
		struct baw_err err;
	} payload;
};

union baw_assetpackmanager_assetpack_update_res {
	struct {
		size_t updatingc;
		const char * const *updatingv;
		size_t removedc;
		const char * const *removedv;
	} success;
	struct baw_err failure;
};

void baw_assetpackmanager_downloadstatusupdates(const char *id, void *ctx, void (*cb)(struct baw_assetpackmanager_downloadstatusupdate downloadstatusupdate, void *ctx));

void baw_assetpackmanager_manifest(void *ctx, void (*cb)(union baw_assetpackmanifest_res res, enum baw_res_kind res_kind, void *ctx));

void baw_assetpackmanager_assetpack_status(struct baw_assetpack assetpack, void *ctx, void (*cb)(union baw_assetpack_status_res res, enum baw_res_kind res_kind, void *ctx));

void baw_assetpackmanager_assetpack_status_local(const char *id, void *ctx, void (*cb)(unsigned char status, void *ctx));

bool baw_assetpackmanager_assetpack_local(const char *id);

void baw_assetpackmanager_assetpack_local_ensure(struct baw_assetpack assetpack, void *ctx, void (*cb)(struct baw_err err, enum baw_res_kind res_kind, void *ctx));

void baw_assetpackmanager_assetpack_local_ensure_update(struct baw_assetpack assetpack, bool update, void *ctx, void (*cb)(struct baw_err err, enum baw_res_kind res_kind, void *ctx));

void baw_assetpackmanager_assetpack_local_ensure_updatev(size_t assetpackc, const struct baw_assetpack *assetpackv, bool update, void *ctx, void (*cb)(struct baw_err err, enum baw_res_kind res_kind, void *ctx));

void baw_assetpackmanager_assetpack_update(void *ctx, void (*cb)(union baw_assetpackmanager_assetpack_update_res res, enum baw_res_kind res_kind, void *ctx));

void baw_assetpackmanager_assetpack_remove(const char *id, void *ctx, void (*cb)(struct baw_err err, enum baw_res_kind res_kind, void *ctx));

struct baw_lang baw_assetpackmanager_lang_resolved(void);

void baw_assetpackmanager_lang_resolved_set(struct baw_lang lang);

void baw_assetpackmanager_lang_local(void *ctx, void (*cb)(size_t langc, const struct baw_lang *langv, void *ctx));

void baw_assetpackmanager_lang_reconcile(void *ctx, void (*cb)(struct baw_err err, enum baw_res_kind res_kind, void *ctx));

signed int baw_assetpackmanager_open(const char *path, const char *id, struct baw_err *err);

signed int baw_assetpackmanager_open_lang(const char *path, struct baw_lang lang, struct baw_err *err);

char *baw_assetpackmanager_url(const char *path, struct baw_err *err);

char *baw_assetpackmanager_url_lang(const char *path, struct baw_lang lang, struct baw_err *err);

void baw_assetpackmanager_url_deinit(char *url);
