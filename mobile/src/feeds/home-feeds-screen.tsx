import React from 'react';
import { FlatList, Text, View, StyleSheet } from 'react-native';
import { TopNavBar } from '../topNavBar/top-nav-bar';

import { Feed } from '../reducers/organization/feed.model';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';

import { FeedListItem } from './feed';

const navBar = new TopNavBar('Feeds');

interface FeedsScreenProps {
    feeds: Feed[];
}

const mapStateToProps = (state: AppState): FeedsScreenProps => ({
    feeds: state.organization.feeds
});

const styles  = StyleSheet.create({
    view: {
        backgroundColor: '#dcdcdc'
    },
    viewHeaderText: {
        fontSize: 12
    }
});

class HomeFeedsScreenImpl extends React.Component<FeedsScreenProps> {
    public static navigationOptions = navBar.configurate();

    public render() {
        return (
            <View style={styles.view}>
                <Text style={styles.viewHeaderText}>News feed</Text>
                <FlatList
                    data = { this.props.feeds }
                    keyExtractor = {this.keyExtractor}
                    renderItem = { ({item}) => <FeedListItem id = { item.messageId } message = { item }/> } />
            </View>
        );
    }

    private keyExtractor = (item: Feed) => item.messageId;
}

export const HomeFeedsScreen = connect(mapStateToProps)(HomeFeedsScreenImpl);