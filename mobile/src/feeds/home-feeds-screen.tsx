import React from 'react';
import { FlatList, View, ListRenderItemInfo } from 'react-native';
import { TopNavBar } from '../navigation/top-nav-bar';

import { Employee } from '../reducers/organization/employee.model';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { Feed } from '../reducers/feeds/feed.model';
import { connect, Dispatch } from 'react-redux';
import { AppState } from '../reducers/app.reducer';

import { FeedListItem } from './feed';
import { LoadingView } from '../navigation/loading';

import { screenStyles as styles } from './styles';
import { openEmployeeDetailsAction } from '../employee-details/employee-details-dispatcher';
import { fetchNewFeeds, fetchOldFeeds } from '../reducers/feeds/feeds.action';
import { loadPhoto } from '../reducers/organization/organization.action';
import { FeedsById } from '../reducers/feeds/feeds.reducer';
import { Moment } from 'moment';

const navBar = new TopNavBar('Feeds');

interface FeedsScreenProps {
    feeds: FeedsById;
    employees: EmployeesStore;
    toDate: Moment;
    fromDate: Moment;
    user: string;
}

interface FeedScreenDispatchProps {
    onAvatarClicked: (employee: Employee) => void;
    fetchNewFeeds: () => void;
    fetchOldFeeds: () => void;
    loadPhoto: (id: string) => void;
}

const mapStateToProps = (state: AppState): FeedsScreenProps => ({
    feeds: state.feeds.feeds,
    employees: state.organization.employees,
    toDate: state.feeds.toDate,
    fromDate: state.feeds.fromDate,
    user: state.userInfo.employeeId,
});

const mapDispatchToProps = (dispatch: Dispatch<any>): FeedScreenDispatchProps => ({
    onAvatarClicked: (employee: Employee) => dispatch(openEmployeeDetailsAction(employee)),
    fetchNewFeeds: () => dispatch(fetchNewFeeds()),
    fetchOldFeeds: () => dispatch(fetchOldFeeds()),
    loadPhoto: (id: string) => dispatch(loadPhoto(id)),
});

class HomeFeedsScreenImpl extends React.Component<FeedsScreenProps & FeedScreenDispatchProps> {
    public static navigationOptions = navBar.configurate();

     public shouldComponentUpdate(nextProps: FeedsScreenProps & FeedScreenDispatchProps) {
        if (this.props.onAvatarClicked !== nextProps.onAvatarClicked
            || this.props.feeds !== nextProps.feeds) {
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
        const feeds = this.sortedFeeds();

        return this.props.feeds.size > 0 ?
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
            if (!employee.photo) {
                this.props.loadPhoto(employee.employeeId);
            }
            return <FeedListItem message={item} employee={employee} onAvatarClicked={this.props.onAvatarClicked} />;
        }
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