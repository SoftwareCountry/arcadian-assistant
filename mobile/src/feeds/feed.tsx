import React from 'react';
import { TouchableHighlight, StyleSheet, Platform, Text, View, Image } from 'react-native';

import { Avatar } from '../people/avatar';

import { Feed } from '../reducers/organization/feed.model';
import { LayoutEvent } from 'react-navigation';

interface FeedListItemProps {
    message: Feed;
    id: string;
}

const styles = StyleSheet.create({
    layout: {
        flex: 1,
        flexDirection: 'row',
        justifyContent: 'center',
        paddingTop: 0,
        paddingBottom: 5
    },
    imgContainer: {
        marginTop: 5,
        flex: 2
    },
    img: {
        flex: 1
    },
    info: {
        flex: 6,
        flexDirection: 'column',
        alignSelf: 'flex-start',
        paddingLeft: 13
    },
    title: {
        fontSize: 19,
        textAlign: 'left',
        fontWeight: '400',
        letterSpacing: 2,
    },
    text: {
        //marginTop: Platform.OS === 'ios' ? 40 : 20,
        fontSize: 15,
        textAlign: 'left',
        paddingTop: 2,
        paddingBottom: 2
    },
    tags: {
        color: '#2FAFCC',
        fontSize: 13
    },
    date: {
        fontSize: 12
    }
});

const months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];

interface IFeedListItemState {
    imgContainerSize: number;
}

export class FeedListItem extends React.Component<FeedListItemProps, IFeedListItemState> {
    public state: IFeedListItemState = {
        imgContainerWidth: 0
    } as any;

    public onLayout = (e: LayoutEvent) => {
        this.setState({
            imgContainerSize: Math.min(e.nativeEvent.layout.width, e.nativeEvent.layout.height)
        });
    }

    public render() {
        const message = this.props.message;
        let date = new Date(this.props.message.datePosted);
        let month = months[date.getMonth()];
        let formattedDate = `${month} ${date.getDate()}, ${date.getFullYear()}`;

        const imgStyle = StyleSheet.flatten([{
            width: this.state.imgContainerSize,
            height: this.state.imgContainerSize
        }]);

        const employee = message.employee;
        const employeeName = employee ? employee.name : message.title;
        const photo = employee ? message.employee.photo : null;
        const mimeType = photo ? photo.mimeType : null;
        const base64 = photo ? photo.base64 : null;

        return (
            <TouchableHighlight>
                <View style={styles.layout}>
                    <View style={styles.imgContainer} onLayout = {this.onLayout}>
                        <Avatar mimeType={mimeType} photoBase64={base64} style={imgStyle}/>
                    </View>
                    <View style={styles.info}>
                        <Text style={styles.title}>{employeeName}</Text>
                        <Text style={styles.text}>{this.props.message.text}</Text>
                        <Text style={styles.tags}>#ArcadiaNews</Text>
                        <Text style={styles.date}>{formattedDate}</Text>
                    </View>
                </View>
            </TouchableHighlight>
        );
    }
}