import React from 'react';
import { FlatList, Text, View, StyleSheet, ListRenderItemInfo } from 'react-native';
import { TopNavBar } from '../topNavBar/top-nav-bar';

import { Employee } from '../reducers/organization/employee.model';
import { Feed } from '../reducers/organization/feed.model';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';

import { FeedListItem } from './feed';

import { screenStyles as styles } from './styles';

const navBar = new TopNavBar('Feeds');

interface FeedsScreenProps {
    feeds: Feed[];
    getEmplooyeeForFeed(feed: Feed): Employee;
}

const mapStateToProps = (state: AppState): FeedsScreenProps => ({
    feeds: state.organization.feeds,
    getEmplooyeeForFeed: (feed: Feed) => {
        return state.organization.employees.employeesById.get(feed.employeeId);
    }
});

class HomeFeedsScreenImpl extends React.Component<FeedsScreenProps> {
    public static navigationOptions = navBar.configurate();

    public render() {
        return (
            <FlatList
                style={styles.view}
                keyExtractor={this.keyExtractor}
                ItemSeparatorComponent={this.itemSeparator}
                data={this.props.feeds}
                renderItem={this.renderItem}
                ListHeaderComponent={this.headerComponent} />
        );
    }

    private keyExtractor(item: Feed) {
        return item ? item.messageId : '';
    }

    private itemSeparator() {
        return <View style={styles.separator}></View>;
    }

    private headerComponent() {
        return <Text style={styles.viewHeaderText}>News feed</Text>;
    }

    private renderItem = (itemInfo: ListRenderItemInfo<Feed>) => {
        const { item } = itemInfo;
        const employee: Employee = this.props.getEmplooyeeForFeed(item);

        return <FeedListItem message={item} employee={employee} />;
    }
}

export const HomeFeedsScreen = connect(mapStateToProps)(HomeFeedsScreenImpl);