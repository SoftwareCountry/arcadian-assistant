import React, {createRef} from 'react';
import {FlatList, ListRenderItemInfo, View} from 'react-native';
import {TopNavBar} from '../navigation/top-nav-bar';

import {Employee} from '../reducers/organization/employee.model';
import {EmployeesStore} from '../reducers/organization/employees.reducer';
import {Feed} from '../reducers/feeds/feed.model';
import {connect, Dispatch} from 'react-redux';
import {AppState} from '../reducers/app.reducer';

import {FeedMessage} from './feed-message';
import {LoadingView} from '../navigation/loading';

import {screenStyles as styles} from './styles';
import {openEmployeeDetailsAction} from '../employee-details/employee-details-dispatcher';
import {fetchNewFeeds, fetchOldFeeds} from '../reducers/feeds/feeds.action';
import {FeedsById} from '../reducers/feeds/feeds.reducer';
import {Moment} from 'moment';
import {NavigationScreenProps} from 'react-navigation';

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
});

class HomeFeedsScreenImpl extends React.Component<NavigationScreenProps & FeedsScreenProps & FeedScreenDispatchProps> {

    public static navigationOptions = navBar.configurate();

    private flatList = createRef<FlatList<any>>();

    public componentDidMount() {
        this.props.navigation.setParams({
            tabBarOnPress: this.scrollToTop
        });
    }

    public render() {
        const feeds = this.sortedFeeds();

        return this.props.feeds.size > 0 ?
            <FlatList ref={this.flatList}
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

    private scrollToTop = () => {
        this.flatList.current.scrollToIndex({index: 0, animated: true});
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
        const {item} = itemInfo;
        const employee: Employee = this.props.employees.employeesById.get(item.employeeId);
        if (!employee) {
            return <FeedMessage message={item} employee={null} onAvatarClicked={this.props.onAvatarClicked}/>;
        } else {
            return <FeedMessage message={item} employee={employee} onAvatarClicked={this.props.onAvatarClicked}/>;
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
