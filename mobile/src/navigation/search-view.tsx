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

interface SearchViewProps {
    type: SearchType;
}

interface SearchViewStateProps {
    filter: string;
    type: SearchType;
}

const mapStateToPropsFeeds = (state: AppState, ownProps: SearchViewProps): SearchViewStateProps => ({
    filter: state.feeds.filter,
    type: ownProps.type
});

const mapStateToPropsPeople = (state: AppState, ownProps: SearchViewProps): SearchViewStateProps => ({
    filter: state.people.filter,
    type: ownProps.type
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
    }

    private changeText(filter: string) {
        this.props.setFilter(filter, this.props.type);
    }
}

const SearchViewFeeds = connect(mapStateToPropsFeeds, mapDispatchToProps)(SearchViewImpl);
const SearchViewPeople = connect(mapStateToPropsPeople, mapDispatchToProps)(SearchViewImpl);

export class SearchView extends Component<SearchViewProps> {
    public render() {
        switch (this.props.type) {
            case SearchType.Feeds:
                return <SearchViewFeeds type={this.props.type}/>;
            case SearchType.People: 
                return <SearchViewPeople type={this.props.type}/>;
            default:
                return <SearchViewPeople type={this.props.type}/>;
        }
    }
}