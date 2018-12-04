import { Nullable, Optional } from 'types';

//----------------------------------------------------------------------------
export function none<T>(object: Nullable<T> | Optional<T>): object is undefined | null {
    return object === null || object === undefined;
}

//----------------------------------------------------------------------------
export function toNullable<T>(optional: Optional<T>): Nullable<T> {
    return optional !== undefined ? optional : null;
}

//----------------------------------------------------------------------------
export function toOptional<T>(nullable: Nullable<T>): Optional<T> {
    return nullable !== null ? nullable : undefined;
}
