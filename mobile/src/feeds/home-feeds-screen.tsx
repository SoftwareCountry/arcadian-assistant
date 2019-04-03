import React from 'react';
import { ActivityIndicator, ListRenderItemInfo, RefreshControl, SafeAreaView, StatusBar, View } from 'react-native';
import { Employee } from '../reducers/organization/employee.model';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { Feed } from '../reducers/feeds/feed.model';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { FeedMessage } from './feed-message';
import { ListStyle, ScreenStyle } from './home-feeds-screen.styles';
import { fetchNewFeeds, fetchOldFeeds } from '../reducers/feeds/feeds.action';
import { FeedsById } from '../reducers/feeds/feeds.reducer';
import { LoadingView } from '../navigation/loading';
import Style from '../layout/style';
import { Action, Dispatch } from 'redux';
import { FlatList } from 'react-navigation';
import { openEmployeeDetails } from '../navigation/navigation.actions';
import { Nullable, Optional } from 'types';

//============================================================================
interface FeedsScreenProps {
    feeds: Optional<FeedsById>;
    employees: Optional<EmployeesStore>;
}

//============================================================================
interface FeedScreenDispatchProps {
    onAvatarClicked: (employee: Employee) => void;
    fetchNewFeeds: () => void;
    fetchOldFeeds: () => void;
}

const mapStateToProps = (state: AppState): FeedsScreenProps => ({
    feeds: state.feeds ? state.feeds.feeds : undefined,
    employees: state.organization ? state.organization.employees : undefined,
});

const mapDispatchToProps = (dispatch: Dispatch<Action>): FeedScreenDispatchProps => ({
    onAvatarClicked: (employee: Employee) => dispatch(openEmployeeDetails(employee.employeeId)),
    fetchNewFeeds: () => dispatch(fetchNewFeeds()),
    fetchOldFeeds: () => dispatch(fetchOldFeeds()),
});

//============================================================================
class HomeFeedsScreenImpl extends React.Component<FeedsScreenProps & FeedScreenDispatchProps> {

    //----------------------------------------------------------------------------
    public render(): React.ReactNode {
        if (!this.isReadyToRenderFeeds(this.props.feeds, this.props.employees)) {
            return this.renderLoading();
        }

        return this.renderFeeds(this.sortedFeeds(this.props.feeds));
    }

    //----------------------------------------------------------------------------
    private renderFeeds(feeds: Feed[]): React.ReactNode {
        return <SafeAreaView style={Style.view.safeArea}>
            <FlatList refreshControl={<RefreshControl refreshing={false} onRefresh={this.onRefresh}/>}
                      style={ScreenStyle.view}
                      keyExtractor={HomeFeedsScreenImpl.keyExtractor}
                      ItemSeparatorComponent={HomeFeedsScreenImpl.itemSeparator}
                      data={feeds}
                      extraData={this.props.employees}
                      renderItem={this.renderItem}
                      onEndReached={this.endReached}
                      onEndReachedThreshold={0.2}
                      ListFooterComponent={this.footer}
                      shouldCancelWhenOutside={false}
            />
        </SafeAreaView>;
    }

    //----------------------------------------------------------------------------
    // noinspection JSMethodCanBeStatic
    private renderLoading(): React.ReactNode {
        return <SafeAreaView style={Style.view.safeArea}>
            <LoadingView/>
        </SafeAreaView>;
    }

    //----------------------------------------------------------------------------
    // noinspection JSMethodCanBeStatic
    private sortedFeeds(feeds: Optional<FeedsById>): Feed[] {
        if (!feeds || !feeds.count()) {
            return Array();
        }

        return feeds.toIndexedSeq().toArray().sort((feed1, feed2) => {
            const dif = feed2.datePosted.valueOf() - feed1.datePosted.valueOf();
            if (dif !== 0) {
                return dif;
            }

            if (!feed1.employeeId || !feed2.employeeId) {
                return 0;
            }

            return feed2.employeeId < feed1.employeeId ? -1 : 1;
        });
    }

    //----------------------------------------------------------------------------
    // noinspection JSMethodCanBeStatic
    private isReadyToRenderFeeds(feeds: Optional<FeedsById>, employees: Optional<EmployeesStore>): boolean {
        return !(!feeds || !feeds.count() || !employees);
    }

    //----------------------------------------------------------------------------
    private footer = (): React.ReactElement<any> => {
        return (
            <View style={ListStyle.footer}>
                <ActivityIndicator color={Style.color.base}/>
            </View>
        );
    };

    //----------------------------------------------------------------------------
    private renderItem = (itemInfo: ListRenderItemInfo<Feed>) => {
        const { item } = itemInfo;

        let employee: Nullable<Employee> = null;
        if (this.props.employees && item.employeeId) {
            employee = this.props.employees.employeesById.get(item.employeeId, null);
        }
        if (!employee) {
            return <FeedMessage message={item}/>;
        }

        return <FeedMessage message={item} employee={employee} onAvatarClicked={this.props.onAvatarClicked}/>;
    };

    //----------------------------------------------------------------------------
    private endReached = () => {
        this.props.fetchOldFeeds();
    };

    //----------------------------------------------------------------------------
    private onRefresh = () => {
        this.props.fetchNewFeeds();
    };

    //----------------------------------------------------------------------------
    private static keyExtractor(item: Feed): string {
        return item ? item.messageId : '';
    }

    //----------------------------------------------------------------------------
    private static itemSeparator(): React.ReactElement<any> {
        return <View style={ScreenStyle.separator}/>;
    }
}

export const HomeFeedsScreen = connect(mapStateToProps, mapDispatchToProps)(HomeFeedsScreenImpl);
