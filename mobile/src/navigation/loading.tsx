import React from 'react';
import { View } from 'react-native';
import { StyledText } from '../override/styled-text';
import { SearchFeedView, SearchPeopleView } from './search-view';
import { searchViewStyles as styles } from './search-view-styles';

export class FeedLoading extends React.Component {
    public render() {
        return (
            <View style={styles.loadingContainer}>
                <View style={styles.searchContainer}>
                    <SearchFeedView/>
                </View>
                <View style={styles.loadingTextContainer}>
                    <StyledText style={styles.loadingText}>Loading...</StyledText>
                </View>
            </View>
        );
    }
}

export class PeopleLoading extends React.Component {
    public render() {
        return (
            <View style={styles.loadingContainer}>
                <View style={styles.searchContainer}>
                    <SearchPeopleView/>
                </View>
                <View style={styles.loadingTextContainer}>
                    <StyledText style={styles.loadingText}>Loading...</StyledText>
                </View>
            </View>
        );
    }
}