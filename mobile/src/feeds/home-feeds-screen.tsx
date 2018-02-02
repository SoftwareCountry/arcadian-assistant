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
    feeds: state.organization.feeds.map(feed => {
        feed.employee = state.organization.employees.employeesById.get(feed.employeeId);
        return feed;
    })
});

const styles = StyleSheet.create({
    view: {
        flex: 1,
        backgroundColor: '#FFF',
        paddingLeft: 19,
        paddingRight: 19
    },
    viewHeaderText: {
        fontSize: 12
    },
    separator: {
        //backgroundColor: '#acacac',
        height: 15
    }
});

class HomeFeedsScreenImpl extends React.Component<FeedsScreenProps> {
    public static navigationOptions = navBar.configurate();

    public render() {
        return (
            <FlatList
                style={styles.view}
                ItemSeparatorComponent={() => <View style={styles.separator}></View>}
                data={this.props.feeds}
                keyExtractor={this.keyExtractor}
                renderItem={({ item }) => <FeedListItem id={item.messageId} message={item} />}
                ListHeaderComponent={() => <Text style={styles.viewHeaderText}>News feed</Text>} />
        );
    }

    private keyExtractor = (item: Feed) => item.messageId;
}

export const HomeFeedsScreen = connect(mapStateToProps)(HomeFeedsScreenImpl);