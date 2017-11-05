import { ActionCreatorsMapObject } from 'redux';

export const Types = {
    EXPAND: 'EXPAND',
    COLLAPSE: 'COLLAPSE',
    SEND: 'SEND',
    LOADLIST: 'LOAD-LIST'
};

export const Actions: ActionCreatorsMapObject = {
    loadList: () => ({
        type: Types.LOADLIST,
    }),
    expand: (id: string) => ({
        type: Types.EXPAND,
        id
    }),
    collapse: (id: string) => ({
        type: Types.COLLAPSE,
        id
    }),
    send: (id: string, payload: string) => ({
        type: Types.SEND,
        id,
        payload
    })
};