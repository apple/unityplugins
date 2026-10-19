//
//  lang.h
//  Background Assets Wrapper
//
//  Created by Gabriel Jacoby-Cooper on 4/20/26.
//  Copyright © 2026 Apple. All rights reserved.
//

#import <stdbool.h>

struct baw_lang {
	void *impl;
};

struct baw_lang baw_lang_init(const char *id);

void baw_lang_deinit(struct baw_lang lang);

bool baw_lang_is_nonnull(struct baw_lang lang);

char *baw_lang_id_min(struct baw_lang lang);

char *baw_lang_id_max(struct baw_lang lang);

void baw_lang_id_deinit(char *lang_id);

bool baw_lang_equivalent(struct baw_lang lang_first, struct baw_lang lang_second);
