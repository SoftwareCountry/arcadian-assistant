import { Action } from 'redux';
import { SearchType } from '../../navigation/search-view';

export interface SetFilter extends Action {
    type: 'SEARCH-BY-TEXT-FILTER';
    filter: string;
    searchType: SearchType;
}

export const startSearch = (filter: string, searchType: SearchType): SetFilter => {
    return { type: 'SEARCH-BY-TEXT-FILTER', filter, searchType };
};

export const endSearch = (searchType: SearchType): SetFilter => {
    return { type: 'SEARCH-BY-TEXT-FILTER', filter: '', searchType};
};

export type SearchActions = SetFilter;