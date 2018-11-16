declare module 'react-native-sensitive-info' {

export function isSensorAvailable(): boolean;

export function setItem(key: string, value: string, options: {}): Promise<string>;
export function getItem(key: string, options: {}): Promise<string>;
export function deleteItem(key: string, options: {}): Promise<string>;
}
