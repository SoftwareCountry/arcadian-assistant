/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

//----------------------------------------------------------------------------
export function capitalizeFirstLetter(text: string) {
    return text.charAt(0).toLocaleUpperCase() + text.slice(1);
}

//----------------------------------------------------------------------------
export function uppercase(text: string | null | undefined): string | null | undefined {
    return text ? text.toLocaleUpperCase() : text;
}
