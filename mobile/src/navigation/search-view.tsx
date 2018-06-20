import React, { Component } from 'react';
import { View, TextInput, Button, TouchableOpacity, Text } from 'react-native';
import { AppState } from '../reducers/app.reducer';
import { connect, Dispatch, MapStateToProps, MapDispatchToPropsFunction } from 'react-redux';
import { setFeedFilter, endFeedSearch } from '../reducers/feeds/feeds.action';
import { setPeopleFilter, endPeopleSearch } from '../reducers/people/people.action';
import { ApplicationIcon } from '../override/application-icon';
import { searchViewStyles as styles } from './search-view-styles';

interface SearchViewState {
    filter: string;
}

interface SearchViewDispatchProps {
    setFilter: (filter: string) => void;
    clearFilter: () => void;
}

const mapDispatchToPropsFeed = (dispatch: Dispatch<any>): SearchViewDispatchProps => ({
    setFilter: (filter) => dispatch(setFeedFilter(filter)),
    clearFilter: () => dispatch(endFeedSearch()),
});

const mapDispatchToPropsPeople = (dispatch: Dispatch<any>): SearchViewDispatchProps => ({
    setFilter: (filter) => dispatch(setPeopleFilter(filter)),
    clearFilter: () => dispatch(endPeopleSearch()),
});

class SearchViewImpl extends Component<SearchViewDispatchProps, SearchViewState> {
    constructor(props: SearchViewDispatchProps) {
        super(props);
        this.state = {
            filter: '',
        };
    }

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
                        value={this.state.filter}
                    />
                </View>
                <View style={styles.iconsContainer}>
                    <TouchableOpacity onPress={this.clearText.bind(this)}>
                        <ApplicationIcon name={'reject-cross'} style={styles.icon} />
                    </TouchableOpacity>
                </View>
            </View>;
    }

    private changeText(filter: string) {
        this.setState({filter});
        this.props.setFilter(filter);
    }

    private clearText() {
        this.setState({filter: ''});
        this.props.clearFilter();
    }
}

export const SearchFeedView = connect(null, mapDispatchToPropsFeed)(SearchViewImpl);
export const SearchPeopleView = connect(null, mapDispatchToPropsPeople)(SearchViewImpl);