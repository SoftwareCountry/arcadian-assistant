import React, { Component } from 'react';
import { View, TextInput, Button, TouchableOpacity, Text } from 'react-native';
import { AppState } from '../reducers/app.reducer';
import { connect, Dispatch, MapStateToProps, MapDispatchToPropsFunction } from 'react-redux';
import { startSearch, endSearch } from '../reducers/search.action';
import { ApplicationIcon } from '../override/application-icon';
import { searchViewStyles as styles } from './search-view-styles';

export enum SearchType {
    'FEED',
    'PEOPLE',
}

interface SearchViewProps {
    type: SearchType;
}

interface SearchViewStateProps {
    filterFeed: string;
    filterPeople: string;
}

const mapStateToProps = (state: AppState): SearchViewStateProps => ({
    filterFeed: state.feeds.filter,
    filterPeople: state.people.filter,
});

interface SearchViewDispatchProps {
    setFilter: (filter: string, type: SearchType) => void;
    clearFilter: (type: SearchType) => void;
}

const mapDispatchToProps = (dispatch: Dispatch<any>): SearchViewDispatchProps => ({
    setFilter: (filter, type) => dispatch(startSearch(filter, type)),
    clearFilter: (type) => dispatch(endSearch(type)),
});

class SearchViewImpl extends Component<SearchViewDispatchProps & SearchViewProps & SearchViewStateProps> {
    public render() {
        return <View style={styles.container}>
                <View style={styles.iconsContainer}>
                    <ApplicationIcon name={'search'} style={styles.icon} />
                </View>
                <View style={styles.inputContainer}>
                    <TextInput 
                        style={styles.input}
                        placeholder = 'Search'
                        placeholderTextColor = 'white'
                        underlineColorAndroid='transparent'
                        autoCapitalize='none'
                        onChangeText={this.changeText.bind(this)}
                        value={this.getCurrentFilter()}
                    />
                </View>
                <View style={styles.iconsContainer}>
                    <TouchableOpacity onPress={this.clearText.bind(this)}>
                        <ApplicationIcon name={'reject-cross'} style={styles.icon} />
                    </TouchableOpacity>
                </View>
            </View>;
    }

    private getCurrentFilter() {
        switch (this.props.type) {
            case SearchType.FEED:
                return this.props.filterFeed;
            case SearchType.PEOPLE:
                return this.props.filterPeople;
            default:
                return '';
        }
    }

    private changeText(filter: string) {
        this.props.setFilter(filter, this.props.type);
    }

    private clearText() {
        this.props.clearFilter(this.props.type);
    }
}

export const SearchView = connect(mapStateToProps, mapDispatchToProps)(SearchViewImpl);