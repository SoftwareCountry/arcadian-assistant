import React from 'react';
import { FlatList, View, StyleSheet, ListRenderItemInfo, RefreshControl,
    TouchableOpacity } from 'react-native';
import { TopNavBar } from '../navigation/top-nav-bar';

import { Employee } from '../reducers/organization/employee.model';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { Feed } from '../reducers/feeds/feed.model';
import { connect, Dispatch } from 'react-redux';
import { AppState } from '../reducers/app.reducer';

import { FeedListItem } from './feed';
import { SearchView, SearchType } from '../navigation/search-view';
import { LoadingView } from '../navigation/loading';

import { screenStyles as styles } from './styles';
import { StyledText } from '../override/styled-text';
import { openEmployeeDetailsAction } from '../employee-details/employee-details-dispatcher';
import { fetchNewFeeds, fetchOldFeeds } from '../reducers/feeds/feeds.action';
import { FeedsById } from '../reducers/feeds/feeds.reducer';
import { Moment } from 'moment';

const navBar = new TopNavBar('Feeds');

interface FeedsScreenProps {
    feeds: FeedsById;
    employees: EmployeesStore;
    toDate: Moment;
    fromDate: Moment;
    user: string;
    filter: string;
    feedPredicate: (feed: Feed) => boolean;
}

interface FeedScreenDispatchProps {
    onAvatarClicked: (employee: Employee) => void;
    fetchNewFeeds: () => void;
    fetchOldFeeds: () => void;
}

const mapStateToProps = (state: AppState): FeedsScreenProps => {
    const filter = state.feeds.filter;
    const employees = state.organization.employees;

    return ({
        feeds: state.feeds.feeds,
        employees,
        toDate: state.feeds.toDate,
        fromDate: state.feeds.fromDate,
        user: state.userInfo.employeeId,
        filter,
        feedPredicate: (feed: Feed) =>  {
            const employee = employees.employeesById.get(feed.employeeId);

            return  feed.title.includes(filter) || 
                    feed.text.includes(filter) ||
                    (employee && employee.name.includes(filter));
        },
    });
};

const mapDispatchToProps = (dispatch: Dispatch<any>): FeedScreenDispatchProps => ({
    onAvatarClicked: (employee: Employee) => dispatch(openEmployeeDetailsAction(employee)),
    fetchNewFeeds: () => dispatch(fetchNewFeeds()),
    fetchOldFeeds: () => dispatch(fetchOldFeeds()),
});

class HomeFeedsScreenImpl extends React.Component<FeedsScreenProps & FeedScreenDispatchProps> {
    public static navigationOptions = navBar.configurate();

     public shouldComponentUpdate(nextProps: FeedsScreenProps & FeedScreenDispatchProps) {
        const currentFeeds = this.props.feeds.filter(this.props.feedPredicate);
        const nextFeeds = nextProps.feeds.filter(nextProps.feedPredicate);

        if (this.props.onAvatarClicked !== nextProps.onAvatarClicked
            || !currentFeeds.equals(nextFeeds)) {
            return true;
        }

        const nothingChanged = nextProps.feeds.every((feed) => {
            const employeeId = feed.employeeId;
            const currentEmployeeRef = this.props.employees.employeesById.get(employeeId);
            const nextEmployeeRef = nextProps.employees.employeesById.get(employeeId);
            return currentEmployeeRef === nextEmployeeRef;
        });

        return !nothingChanged;
    } 

    public render() {
        const feeds = this.sortedFeeds().filter(this.props.feedPredicate);

        return this.props.feeds.size > 0 ?
            <View>
                <FlatList
                    style={styles.view}
                    keyExtractor={this.keyExtractor}
                    ItemSeparatorComponent={this.itemSeparator}
                    data={feeds}
                    extraData={this.props.employees}
                    renderItem={this.renderItem}
                    onEndReached={this.endReached}
                    onEndReachedThreshold={0}
                    refreshing={false}
                    onRefresh={this.onRefresh}
                />
            </View>
        : <LoadingView/>;
    }

    private keyExtractor(item: Feed) {
        return item ? item.messageId : '';
    }

    private itemSeparator() {
        return <View style={styles.separator}></View>;
    }

    private sortedFeeds() {
        return this.props.feeds.toArray().sort((x, y) => {
            return ((y.datePosted.valueOf() - x.datePosted.valueOf()) || (y.employeeId < x.employeeId ? -1 : 1));
        });
    }

    private renderItem = (itemInfo: ListRenderItemInfo<Feed>) => {
        const { item } = itemInfo;
        const employee: Employee = this.props.employees.employeesById.get(item.employeeId);
        if (!employee) {
            return <FeedListItem message={item} employee={null} onAvatarClicked={this.props.onAvatarClicked} />;
        } else {
            return <FeedListItem message={item} employee={employee} onAvatarClicked={this.props.onAvatarClicked} />;
        }
    }

    private renderFooter = () => {
        return <View style={styles.footer}>
                <TouchableOpacity onPress={this.endReached}>
                    <StyledText>
                        Load more
                    </StyledText>
                </TouchableOpacity>
            </View>;
    }

    private endReached = () => {
        if (this.props.user) {
            this.props.fetchOldFeeds();
        }
    }

    private onRefresh = () => {
        this.props.fetchNewFeeds();
    }
}

export const HomeFeedsScreen = connect(mapStateToProps, mapDispatchToProps)(HomeFeedsScreenImpl);