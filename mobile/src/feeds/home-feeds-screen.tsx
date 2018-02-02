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

interface FeedListItemInfo {
    feed: Feed;
    employee: Employee;
    key: string;
}

interface FeedsScreenProps {
    feeds: FeedListItemInfo[];
}

const mapStateToProps = (state: AppState): FeedsScreenProps => ({
    feeds: state.organization.feeds.map(feed => ({
        feed: feed,
        employee: state.organization.employees.employeesById.get(feed.employeeId),
        key: feed.employeeId
    }))
});

class HomeFeedsScreenImpl extends React.Component<FeedsScreenProps> {
    public static navigationOptions = navBar.configurate();

    public render() {
        return (
            <FlatList
                style={styles.view}
                ItemSeparatorComponent={() => <View style={styles.separator}></View>}
                data={this.props.feeds}
                renderItem={this.renderItem}
                ListHeaderComponent={() => <Text style={styles.viewHeaderText}>News feed</Text>} />
        );
    }

    private renderItem( itemInfo: ListRenderItemInfo<FeedListItemInfo> ) {
        const {item} = itemInfo;
        return <FeedListItem id={item.key} message={item.feed} employee={item.employee}/>;
    }
}

export const HomeFeedsScreen = connect(mapStateToProps)(HomeFeedsScreenImpl);