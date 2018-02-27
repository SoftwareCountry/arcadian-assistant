import React from 'react';
import { FlatList, View, StyleSheet, ListRenderItemInfo, RefreshControl } from 'react-native';
import { TopNavBar } from '../topNavBar/top-nav-bar';

import { Employee } from '../reducers/organization/employee.model';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { Feed } from '../reducers/feeds/feed.model';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';

import { FeedListItem } from './feed';

import { screenStyles as styles } from './styles';
import { StyledText } from '../override/styled-text';

const navBar = new TopNavBar('Feeds');

interface FeedsScreenProps {
    feeds: Feed[];
    employees: EmployeesStore;
}

const mapStateToProps = (state: AppState): FeedsScreenProps => ({
    feeds: state.feeds,
    employees: state.organization.employees
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
                extraData={this.props.employees}
                renderItem={this.renderItem} />
        );
    }

    private keyExtractor(item: Feed) {
        return item ? item.messageId : '';
    }

    private itemSeparator() {
        return <View style={styles.separator}></View>;
    }

    private renderItem = (itemInfo: ListRenderItemInfo<Feed>) => {
        const { item } = itemInfo;
        const employee: Employee = this.props.employees.employeesById.get(item.employeeId);

        return <FeedListItem message={item} employee={employee} />;
    }
}

export const HomeFeedsScreen = connect(mapStateToProps)(HomeFeedsScreenImpl);