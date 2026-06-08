//
//  err.h
//  Background Assets Wrapper
//
//  Created by Gabriel Jacoby-Cooper on 4/20/26.
//  Copyright © 2026 Apple. All rights reserved.
//

#import <stdbool.h>
#import <stddef.h>

union baw_err_description {
	const unsigned char *str_static;
	char *str;
};

struct baw_err {
	union baw_err_description description;
	bool _static;
};

struct baw_err_localavailability {
	struct baw_err base;
	size_t successc;
	const char * const *success_idv;
	size_t failurev;
	const char * const *failure_idv;
	struct baw_err *failure_errv;
};

void baw_err_deinit(struct baw_err err);
