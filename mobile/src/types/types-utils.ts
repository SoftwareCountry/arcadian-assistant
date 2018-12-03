import { Nullable, Optional } from 'types';

//----------------------------------------------------------------------------
export function none<T>(object: Nullable<T> | Optional<T>): object is undefined | null {
    return object === null || object === undefined;
}
