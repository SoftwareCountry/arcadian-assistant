declare module 'react-native-fingerprint-scanner' {

    export interface Error {
        name: string;
        message: string;
    }

    export interface OnAttemptObject {
        onAttempt?: (error: Error) => void;
        description?: string;
    }

    export function isSensorAvailable(): Promise<string>;

    export function authenticate(onAttempt: OnAttemptObject): Promise<void>;

    export function release(): void;
}
