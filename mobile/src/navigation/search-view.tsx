import React, { Component } from 'react';
import { View, TextInput, Button, TouchableOpacity, Text } from 'react-native';
import { AppState } from '../reducers/app.reducer';
import { connect, Dispatch, MapStateToProps, MapDispatchToPropsFunction } from 'react-redux';
import { startSearch, endSearch } from '../reducers/search.action';
import { ApplicationIcon } from '../override/application-icon';
import { searchViewStyles as styles } from './search-view-styles';

export enum SearchType {
    Feeds = 'Feeds',
    People = 'People',
}

interface SearchViewState {
    isActive: boolean;
}

interface SearchViewStateProps {
    filter: string;
    type: SearchType;
}

const mapStateToPropsFeeds = (state: AppState): SearchViewStateProps => ({
    filter: state.feeds.filter,
    type: SearchType.Feeds
});

const mapStateToPropsPeople = (state: AppState): SearchViewStateProps => ({
    filter: state.people.filter,
    type: SearchType.People
});

interface SearchViewDispatchProps {
    setFilter: (filter: string, type: SearchType) => void;
    clearFilter: (type: SearchType) => void;
}

const mapDispatchToProps = (dispatch: Dispatch<any>): SearchViewDispatchProps => ({
    setFilter: (filter, type) => dispatch(startSearch(filter, type)),
    clearFilter: (type) => dispatch(endSearch(type)),
});

class SearchViewImpl extends Component<SearchViewDispatchProps & SearchViewStateProps, SearchViewState> {
    constructor(props: SearchViewDispatchProps & SearchViewStateProps) {
        super(props);
        this.state = {
            isActive: false,
        };
    }

    public render() {
        return <View style={styles.container}>
                <TouchableOpacity
                    style={styles.iconsContainer}
                    onPress={this.onPress.bind(this)}
                >
                    <ApplicationIcon 
                        name={'search'} 
                        style={this.state.isActive ? styles.activeIcon : styles.inactiveIcon}
                    />
                </TouchableOpacity>
                <View style={styles.inputContainer}>
                    {this.state.isActive ? 
                    <TextInput 
                        style={styles.input}
                        underlineColorAndroid='transparent'
                        autoCapitalize='none'
                        onChangeText={this.changeText.bind(this)}
                        value={this.props.filter}
                    /> : null}
                </View>
            </View>;
    }

    private onPress() {
        this.setState({isActive: !this.state.isActive});
        this.props.clearFilter(this.props.type);
    }

    private changeText(filter: string) {
        this.props.setFilter(filter, this.props.type);
    }
}

export const SearchViewPeople = connect(mapStateToPropsPeople, mapDispatchToProps)(SearchViewImpl);